﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machina.Types.Geometry;

namespace Machina.Drivers.Communication.Protocols
{
    class URCommunicationProtocol : Base
    {
        // (not implemented yet)
        internal static readonly char STR_MESSAGE_END_CHAR = ';';
        internal static readonly char STR_MESSAGE_ID_CHAR = '@';
        internal static readonly char STR_MESSAGE_RESPONSE_CHAR = '>';

        internal static readonly string MACHINA_SERVER_VERSION = "1.1.1";

        // Instruction data will be sent to the socket in the form of 32 signed integers. 
        // To allow for float precision, the original values must be 'puffed' by these factors. 
        // This works de facto as the maximum precision for unit value types.
        internal const double FACTOR_M = 10000.0;
        internal const double FACTOR_RAD = 10000.0;
        internal const double FACTOR_SEC = 1000.0;
        internal const double FACTOR_KG = 1000.0;
        internal const double FACTOR_VOLT = 1000000.0;

        // Instruction codes. 
        // Instruction buffers start with an ID that will be sent back on the acknowledgement response (use -1 if not interested),
        // then a numeric code which determines the instruction to perform, and variable number of parameters for the instruction.
        // Please note that, with the exception of strings (WIP), all parameters must be integers that have been premultiplied by
        // their corresponding unit factor (see above).
        // INCOMING BUFFER:
        internal const int INST_MOVEL = 1;                   // [ID, CODE, X, Y, Z, RX, RY, RZ] (in (int) M * FACTOR_M, RAD * FACTOR_RAD)
        internal const int INST_MOVEJ_P = 2;                 // [ID, CODE, X, Y, Z, RX, RY, RZ] (in (int) M * FACTOR_M, RAD * FACTOR_RAD)
        internal const int INST_MOVEJ_Q = 3;                 // [ID, CODE, J1, J2, J3, J4, J5, J6] (in (int) RAD * FACTOR_RAD)
        internal const int INST_TCP_SPEED = 4;               // [ID, CODE, VEL] (in (int) M/S * FACTOR_M)
        internal const int INST_TCP_ACC = 5;                 // [ID, CODE, ACC] (in (int) M/S^2 * FACTOR_M)
        internal const int INST_Q_SPEED = 6;                 // [ID, CODE, VEL] (in (int) RAD/S * FACTOR_RAD)
        internal const int INST_Q_ACC = 7;                   // [ID, CODE, ACC] (in (int) RAD/S^2 * FACTOR_RAD)
        internal const int INST_BLEND = 8;                   // [ID, CODE, RADIUS] (in (int) M * FACTOR_M)
        internal const int INST_SLEEP = 9;                   // [ID, CODE, TIME] (in (int) S * FACTOR_SEC)
        //   internal const int INST_TEXTMSG = 10                 // [ID, CODE, MSG] (in (string) "msg" + STR_MESSAGE_END_CHAR) (NOT IMPLEMENTED)
        //   internal const int INST_POPUP = 11                   // [ID, CODE, MSG] (in (string) "msg" + STR_MESSAGE_END_CHAR) (NOT IMPLEMENTED)
        internal const int INST_SET_TOOL = 12;               // [ID, CODE, X, Y, Z, RX, RY, RZ, KG] (in (int) M * FACTOR_M, RAD * FACTOR_RAD, KG * FACTOR_KG)
        internal const int INST_SET_DIGITAL_OUT = 13;        // [ID, CODE, PIN, ON, TOOL] (in (int), bool) 
        internal const int INST_SET_ANALOG_OUT = 14;         // [ID, CODE, PIN, VOLTAGE] (in (int) VOLTAGE * FACTOR_VOLT) (there is no analog out on the tool)
        // This value sets the same speed value for TCP and Q, taken as mm/s and deg/s. E.g: if setting to 20 mm/s or deg/s, the received value should be 0.02 m/s * FACTOR_M, and this will be converted internally to 0.349 rad/s (=20 deg/s)
        internal const int INST_ALL_SPEED = 15;              // [ID, CODE, VEL] (in (int) M/S * FACTOR_M)
        // Similarly here, a received value in "puffed" m/s^2 will be internally translated to rad/s^2
        internal const int INST_ALL_ACC = 16;                // [ID, CODE, ACC] (in (int) M/S^2 * FACTOR_M)
        internal const int INST_MOVEP = 17;                  // [ID, CODE, X, Y, Z, RX, RY, RZ] (in (int) M * FACTOR_M, RAD * FACTOR_RAD)
        
        // Gripper stuff
        internal const int INST_ONROBOT_RG6 = 27;                  

        // ScrewDriver stuff
        internal const int INST_ONROBOT_SD_SHANK = 28;
        internal const int INST_ONROBOT_SD_TIGHTEN = 29;
        internal const int INST_ONROBOT_SD_LOOSEN = 30;
        internal const int INST_ONROBOT_SD_PREMOUNT = 31;
        internal const int INST_ONROBOT_SD_PICK_SCREW = 32;
        internal const int INST_ONROBOT_VG_CHANNELGRIP = 33;
        internal const int INST_ONROBOT_VG_GRIPALL = 34;
        internal const int INST_ONROBOT_VG_RELEASE = 35;

        internal const int RES_FULL_POSE = -54;              // ">54 X Y Z RX RY RZ J1 J2 J3 J4 J5 J6;" Sends all pose and joint info
        internal const int RES_END = -2147483648;            // Used to denote the end of sending messages

        // With older UR robots, I was getting a problem where the robot would
        // not move (or even crash completely) if issuing an action that results in the 
        // robot on the same target position, like a `Rotate` or moving the 6th axis.
        // A quick workaround is to set precision to 0 before the action, 
        // and reset it afterwards.
        // https://github.com/RobotExMachina/Machina.NET/issues/7
        private const bool ZERO_PRECISION_ON_SAME_POSITION_MOTION = true;
        

        // For compilation reuse
        private byte[] _buffer;
        private int[] _params = null;
        private Action _action;

        /// <summary>
        /// Given an Action and a RobotCursor representing the state of the robot after application, 
        /// return a List of strings with the messages necessary to perform this Action adhering to 
        /// the Machina-ABB-Server protocol.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        internal override List<string> GetActionMessages(Action action, RobotCursor cursor)
        {
            //List<string> msgs = new List<string>();
            //string dec;
            //if (!CompilerUR.GenerateInstructionDeclaration(action, cursor, false, out dec))
            //    return null;

            //// The message type in the response is currently ignored by Machina, but let's send a zero anyway to not break string splitting and other checks... 
            //string res = $"  socket_send_string(\"{STR_MESSAGE_ID_CHAR}{action.id} 0{STR_MESSAGE_END_CHAR}\")";

            //msgs.Add(dec);
            //msgs.Add(res);
            //return msgs;
            return null;
        }

        public override byte[] GetBytesForNextAction(RobotCursor cursor)
        {
            if (!cursor.ApplyNextAction(out _action)) return null;  // cursor buffer is empty

            _params = null;

            switch (_action.Type)
            {

                case ActionType.OnrobotRG6:
                    ActionRG6Gripper rbg = _action as ActionRG6Gripper;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_RG6,
                        (int) cursor.gripperDistance,
                        (int) cursor.gripForce,
                        (int) rbg.waitTime,
                    };
                    break;
                case ActionType.OnrobotSD_shank:
                    ActionOnRobotScrewDriverShank sdShank = _action as ActionOnRobotScrewDriverShank;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_SD_SHANK,
                        (int) cursor.screwDriverShank,
                        (int) sdShank.wait_time
                    };
                    break;
                case ActionType.OnrobotSD_tighten:
                    ActionOnRobotSD_Tighten sdTighten = _action as ActionOnRobotSD_Tighten;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_SD_TIGHTEN,
                        (int) sdTighten.screwLength,
                        (int) sdTighten.torque,
                        (int) sdTighten.wait_time
                    };
                    break;
                case ActionType.OnRobotSD_loosen:
                    ActionOnRobotSD_Loosen sdLoosen = _action as ActionOnRobotSD_Loosen;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_SD_LOOSEN,
                        (int) sdLoosen.screwLength,
                        (int) sdLoosen.wait_time
                    };
                    break;
                case ActionType.OnRobotSD_Premount:
                    ActionOnRobotSD_Premount sdPremount = _action as ActionOnRobotSD_Premount;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_SD_PREMOUNT,
                        (int) sdPremount.screwLength,
                        (int) sdPremount.torque,
                        (int) sdPremount.wait_time
                    };
                    break;
                case ActionType.OnrobotSD_PickScrew:
                    ActionOnRobotSD_PickScrew sdPickScrew = _action as ActionOnRobotSD_PickScrew;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_SD_PICK_SCREW,
                        (int) sdPickScrew.screwLength,
                        (int) sdPickScrew.wait_time
                    };
                    break;
                case ActionType.OnRobotVG_ChannelGrip:
                    ActionOnRobotVG_ChannelGrip vgChannelGrip = _action as ActionOnRobotVG_ChannelGrip;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_VG_CHANNELGRIP,
                        (int) vgChannelGrip.channel01,
                        (int) vgChannelGrip.channel02,
                        (int) vgChannelGrip.power_limit,
                        (int) vgChannelGrip.wait_time
                    };
                    break;
                case ActionType.OnRobotVG_GripAll:
                    ActionOnRobotVG_GripAll vgGripAll = _action as ActionOnRobotVG_GripAll;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_VG_GRIPALL,
                        (int) vgGripAll.channels,
                        (int) vgGripAll.power_limit,
                        (int) vgGripAll.wait_time
                    };
                    break;
                case ActionType.OnRobotVG_Release:
                    ActionOnRobotVG_Release vgRelease = _action as ActionOnRobotVG_Release;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ONROBOT_VG_RELEASE,
                        (int) vgRelease.wait_time
                    };
                    break;
                case ActionType.Translation:
                case ActionType.Rotation:
                case ActionType.Transformation:  
                    RotationVector rv = cursor.rotation.AA.ToRotationVector();
                    _params = new int[]
                    {
                        _action.Id,
                        cursor.motionType == MotionType.Joint ? INST_MOVEJ_P : INST_MOVEL,
                        (int) Math.Round(cursor.position.X * 0.001 * FACTOR_M),
                        (int) Math.Round(cursor.position.Y * 0.001 * FACTOR_M),
                        (int) Math.Round(cursor.position.Z * 0.001 * FACTOR_M),
                        (int) Math.Round(rv.X * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(rv.Y * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(rv.Z * Geometry.TO_RADS * FACTOR_RAD)
                    };

                    // Workaround to SW3.0 crash problem: https://github.com/RobotExMachina/Machina.NET/issues/7
                    if (ZERO_PRECISION_ON_SAME_POSITION_MOTION && cursor.precision != 0 && cursor.prevPosition != null && cursor.position.IsSimilar(cursor.prevPosition))
                    {
                        Logger.Debug("Applying ZERO_PRECISION_ON_SAME_POSITION_MOTION");
                        _params = WrapParamsWithZeroPrecision(_params, cursor);
                    }

                    break;

                case ActionType.Axes:
                    _params = new int[]
                    {
                        _action.Id,
                        INST_MOVEJ_Q,
                        (int) Math.Round(cursor.axes.J1 * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(cursor.axes.J2 * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(cursor.axes.J3 * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(cursor.axes.J4 * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(cursor.axes.J5 * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(cursor.axes.J6 * Geometry.TO_RADS * FACTOR_RAD),
                    };

                    // Another ZERO_PRECISION_ON_SAME_POSITION_MOTION check should happen here for 6th axis rotation only...
                    
                    break;

                case ActionType.Wait:
                    ActionWait aa = _action as ActionWait;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_SLEEP,
                        (int) Math.Round(aa.millis * 0.001 * FACTOR_SEC)
                    };
                    break;

                // Not implemented yet
                //case ActionType.Message:
                //    ActionMessage am = (ActionMessage)action;
                //    dec = string.Format("  popup(\"{0}\", title=\"Machina Message\", warning=False, error=False)",
                //        am.message);
                //    break;

                case ActionType.AttachTool:
                    //ActionAttachTool aatt = _action as ActionAttachTool;
                    //Tool t = aatt.tool;

                    Tool t = cursor.tool;  // can cursor.tool be null? The action would have not gone through if there wasn't a tool available for attachment, right?
                    RotationVector trv = t.TCPOrientation.ToRotationVector();
                    _params = new int[]
                    {
                        _action.Id,
                        INST_SET_TOOL,
                        (int) Math.Round(t.TCPPosition.X * 0.001 * FACTOR_M),
                        (int) Math.Round(t.TCPPosition.Y * 0.001 * FACTOR_M),
                        (int) Math.Round(t.TCPPosition.Z * 0.001 * FACTOR_M),
                        (int) Math.Round(trv.X * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(trv.Y * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(trv.Z * Geometry.TO_RADS * FACTOR_RAD),
                        (int) Math.Round(t.Weight * FACTOR_KG)
                    };
                    break;

                case ActionType.DetachTool:
                    _params = new int[]
                    {
                        _action.Id,
                        INST_SET_TOOL,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0
                    };
                    break;

                case ActionType.IODigital:
                    ActionIODigital aiod = _action as ActionIODigital;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_SET_DIGITAL_OUT,
                        aiod.pinNum,
                        aiod.on ? 1 : 0,
                        aiod.isToolPin ? 1 : 0
                    };
                    break;

                case ActionType.IOAnalog:
                    ActionIOAnalog aioa = _action as ActionIOAnalog;
                    _params = new int[]
                    {
                        _action.Id,
                        INST_SET_ANALOG_OUT,
                        aioa.pinNum,
                        (int) Math.Round(aioa.value * FACTOR_VOLT)
                        //aioa.isToolPin ? 1 : 0  // there is no analog out on the tool
                    };
                    break;


                //  ╔═╗╔═╗╔╦╗╔╦╗╦╔╗╔╔═╗╔═╗
                //  ╚═╗║╣  ║  ║ ║║║║║ ╦╚═╗
                //  ╚═╝╚═╝ ╩  ╩ ╩╝╚╝╚═╝╚═╝
                // Speed is now set globally, and the driver takes care of translating that internally
                case ActionType.Speed:
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ALL_SPEED,
                        (int) Math.Round(cursor.speed * 0.001 * FACTOR_M)
                    };
                    break;

                // Idem for acceleration
                case ActionType.Acceleration:
                    _params = new int[]
                    {
                        _action.Id,
                        INST_ALL_ACC,
                        (int) Math.Round(cursor.acceleration * 0.001 * FACTOR_M)
                    };
                    break;

                case ActionType.Precision:
                    _params = new int[]
                    {
                        _action.Id,
                        INST_BLEND,
                        (int) Math.Round(cursor.precision * 0.001 * FACTOR_M)
                    };
                    break;

                case ActionType.PushPop:
                    ActionPushPop app = _action as ActionPushPop;
                    if (app.push) break;

                    Settings beforePop = cursor.settingsBuffer.SettingsBeforeLastPop;
                    Dictionary<int, int> poppedSettings = new Dictionary<int, int>();

                    // These are the states kept in the controller as of v1.0 of the driver
                    //if (beforePop.Speed != cursor.speed)
                    //    poppedSettings.Add(INST_TCP_SPEED, (int)Math.Round(cursor.speed * 0.001 * FACTOR_M));

                    //if (beforePop.Acceleration != cursor.acceleration)
                    //    poppedSettings.Add(INST_TCP_ACC, (int)Math.Round(cursor.acceleration * 0.001 * FACTOR_M));

                    //if (beforePop.JointSpeed != cursor.jointSpeed)
                    //    poppedSettings.Add(INST_Q_SPEED, (int)Math.Round(cursor.jointSpeed * Geometry.TO_RADS * FACTOR_RAD));

                    //if (beforePop.JointAcceleration != cursor.jointAcceleration)
                    //    poppedSettings.Add(INST_Q_ACC, (int)Math.Round(cursor.jointAcceleration * Geometry.TO_RADS * FACTOR_RAD));

                    // These are the states kept in the controller as of v1.0 of the driver
                    if (beforePop.Speed != cursor.speed)
                        poppedSettings.Add(INST_ALL_SPEED, (int)Math.Round(cursor.speed * 0.001 * FACTOR_M));

                    if (beforePop.Acceleration != cursor.acceleration)
                        poppedSettings.Add(INST_ALL_ACC, (int)Math.Round(cursor.acceleration * 0.001 * FACTOR_M));

                    if (beforePop.Precision != cursor.precision)
                        poppedSettings.Add(INST_BLEND, (int)Math.Round(cursor.precision * 0.001 * FACTOR_M));

                    // Generate a buffer with all instructions, ids of -1 except for the last one.
                    _params = new int[3 * poppedSettings.Count];
                    int it = 0;
                    foreach (var setting in poppedSettings)
                    {
                        _params[3 * it] = it == poppedSettings.Count - 1 ? app.Id : -1;  // only attach the real id to the last instruction
                        _params[3 * it + 1] = setting.Key;
                        _params[3 * it + 2] = setting.Value;
                        it++;
                    }

                    break;

                case ActionType.Coordinates:
                    throw new NotImplementedException();  // @TODO: this should also change the WObj, but not on it yet...

                //// Send comma-separated integers
                //case ActionType.CustomCode:
                //    ActionCustomCode acc = _action as ActionCustomCode;
                //    int[] values;
                //    if (Numeric.CommaSeparatedStringToInts(out values))
                //    {

                //    }
                //    else
                //    {
                //        Logger.Warning("Invalid CustomCode: please use a string of comma-separated integers, like \"1,);
                //    }
                    
                //    break;

                // If the Action wasn't on the list above, it doesn't have a message representation...
                default:
                    Logger.Verbose("Cannot stream action `" + _action + "`");
                    return null;

            }

            if (_params == null) return null;

            _buffer = Utilities.Conversion.Int32ArrayToByteArray(_params, false);

            return _buffer;
        }

        /// <summary>
        /// Workaround to SW3.0 crash problem: https://github.com/RobotExMachina/Machina.NET/issues/7
        /// For such motions, make precision zero temporarily.
        /// </summary>
        /// <param name="_params"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        private int[] WrapParamsWithZeroPrecision(int[] _params, RobotCursor cursor)
        {
            int len = 6 + _params.Length;

            int[] wrapParam = new int[len];

            wrapParam[0] = 0;  // no id
            wrapParam[1] = INST_BLEND;
            wrapParam[2] = 0;

            _params.CopyTo(wrapParam, 3);

            wrapParam[len - 3] = 0;
            wrapParam[len - 2] = INST_BLEND;
            wrapParam[len - 1] = (int) Math.Round(cursor.precision * 0.001 * FACTOR_M);

            return wrapParam;
        }


    }
}
