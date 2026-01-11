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
    public class ActionOnRobotSD_Tighten : Action
    {
        public int screwLength;
        public int torque;
        public int wait_time;

        

        public override ActionType Type => ActionType.OnrobotSD_tighten;

        public ActionOnRobotSD_Tighten(int screwLength, int torque, int wait_time) : base()
        {
            this.screwLength = screwLength;
            this.torque = torque;
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Screw Driver Tighten a {0}mm screw with {1}Nm of power_limit with a {2} millisecond pause",
                this.screwLength,
                this.torque / 100.0,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {

            string screwType = "";
            foreach (string key in OnRobotDefaults.OnRobotScewTypes.Keys)
            {
                if (OnRobotDefaults.OnRobotScewTypes[key] == this.torque) 
                {
                    screwType = key;
                    break;
                }
            }

            return string.Format("SD_Tighten({0},{1},{2});",
                this.screwLength,
                screwType,
                this.wait_time
            );

        }
    }
}