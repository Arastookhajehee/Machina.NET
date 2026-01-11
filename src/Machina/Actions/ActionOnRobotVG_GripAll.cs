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
    public class ActionOnRobotVG_GripAll : Action
    {
        public int channels;
        public int power_limit;
        public int wait_time;



        public override ActionType Type => ActionType.OnRobotVG_GripAll;

        public ActionOnRobotVG_GripAll(int channels, int power_limit, int wait_time) : base()
        {

            power_limit = power_limit < 100 ? 100 : power_limit;
            power_limit = power_limit > 1000 ? 1000 : power_limit;

            channels = channels < 5 ? 5 : channels;
            channels = channels > 80 ? 80 : channels;

            this.channels = channels;
            this.power_limit = power_limit;
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Vaccum Gripper All Channels to {0}, with {1} power_limit, and waiting for {2} milliseconds",
                this.channels,
                this.power_limit,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {
            return string.Format("VG10_GripAll({0},{1},{2});",
                this.channels,
                this.power_limit,
                this.wait_time
            );

        }
    }
}