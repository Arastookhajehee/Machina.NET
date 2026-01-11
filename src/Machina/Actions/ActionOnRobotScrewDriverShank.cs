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
    public class ActionOnRobotScrewDriverShank : Action
    {
        public int shankPosition;
        public int wait_time;

        public override ActionType Type => ActionType.OnrobotSD_shank;

        public ActionOnRobotScrewDriverShank(int shankPosition, int wait_time) : base()
        {
            this.shankPosition = shankPosition;
            this.wait_time = wait_time; 
        }

        public override string ToString()
        {

            return string.Format("Set OnRobot Screw Driver Shank Position to {0}mm with a {1} millisecond pause",
                this.shankPosition,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {

            return string.Format("SD_ShankTo({0},{1});",
                this.shankPosition,
                this.wait_time
            );

        }
    }
}