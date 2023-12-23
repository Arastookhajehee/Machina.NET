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
    public class ActionOnRobotSD_Loosen : Action
    {
        public int screwLength;
        public int wait_time;

        public override ActionType Type => ActionType.OnRobotSD_loosen;

        public ActionOnRobotSD_Loosen(int screwLength, int wait_time) : base()
        {
            this.screwLength = screwLength;
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Screw Driver Loosen a {0}mm screw with a {1} millisecond pause",
                this.screwLength,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {

            return string.Format("SD_Loosen({0},{1});",
                this.screwLength,
                this.wait_time
            );

        }
    }
}