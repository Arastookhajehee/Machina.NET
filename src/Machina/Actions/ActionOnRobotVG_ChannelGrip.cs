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
    public class ActionOnRobotVG_ChannelGrip : Action
    {
        public int channel01;
        public int channel02;
        public int power_limit;
        public int wait_time;



        public override ActionType Type => ActionType.OnRobotVG_ChannelGrip;

        public ActionOnRobotVG_ChannelGrip(int channel01, int channel02, int power_limit, int wait_time) : base()
        {

            power_limit = power_limit < 100 ? 100 : power_limit;
            power_limit = power_limit > 1000 ? 1000 : power_limit;

            channel01 = channel01 < 5 ? 5 : channel01;
            channel01 = channel01 > 80 ? 80 : channel01;

            channel02 = channel02 < 5 ? 5 : channel02;
            channel02 = channel02 > 80 ? 80 : channel02;


            this.channel01 = channel01;
            this.channel02 = channel02;
            this.power_limit = power_limit;
            this.wait_time = wait_time;
        }

        public override string ToString()
        {

            return string.Format("OnRobot Vaccum Gripper Channel 1 to {0}, channel 2 to {1}, with {2} power_limit, and waiting for {3} milliseconds",
                this.channel01,
                this.channel02,
                this.power_limit,
                this.wait_time
                );
        }


        public override string ToInstruction()
        {
            return string.Format("VG10_ChannelGrip({0},{1},{2},{3});",
                this.channel01,
                this.channel02,
                this.power_limit,
                this.wait_time
            );

        }
    }
}