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
    public class ActionOnRobotVG_Release : Action
    {
        public int wait_time;



        public override ActionType Type => ActionType.OnRobotVG_Release;

        public ActionOnRobotVG_Release(int wait_time) : base()
        {
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Vaccum Gripper Release all channels, waiting for {0} milliseconds",
                this.wait_time
                );
        }


        public override string ToInstruction()
        {
            return string.Format("VG10_Release({0});",
                this.wait_time
            );

        }
    }
}