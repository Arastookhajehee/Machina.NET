using Machina.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Machina
{

    //   ██████╗ ██████╗ ██╗██████╗ ██████╗ ███████╗██████╗ 
    //  ██╔════╝ ██╔══██╗██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
    //  ██║  ███╗██████╔╝██║██████╔╝██████╔╝█████╗  ██████╔╝
    //  ██║   ██║██╔══██╗██║██╔═══╝ ██╔═══╝ ██╔══╝  ██╔══██╗
    //  ╚██████╔╝██║  ██║██║██║     ██║     ███████╗██║  ██║
    //   ╚═════╝ ╚═╝  ╚═╝╚═╝╚═╝     ╚═╝     ╚══════╝╚═╝  ╚═╝

    /// <summary>
    /// An Action to control the Onrobot Screw Driver Shank.  @Developed by Arastoo Khajehee 
    /// </summary>
    public class ActionOnRobotSD_Premount : Action
    {
        public int screwLength;
        public int torque;
        public int wait_time;

        public override ActionType Type => ActionType.OnRobotSD_Premount;

        public ActionOnRobotSD_Premount(int screwLength, int torque, int wait_time) : base()
        {
            torque = torque < 17 ? 17 : torque;
            torque = torque > 500 ? 500 : torque;

            this.screwLength = screwLength;
            this.torque = torque;
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Screw Driver Premount a {0}mm screw with {1}Nm of power_limit with a {2} millisecond pause",
                this.screwLength,
                this.torque / OnRobotDefaults.tourqueScaleRatio,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {

            return string.Format("SD_Premount({0},{1},{2});",
                this.screwLength,
                this.torque / OnRobotDefaults.tourqueScaleRatio,
                this.wait_time
            );

        }
    }
}