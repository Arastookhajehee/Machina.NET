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
    /// An Action to control the Onrobot RG6 Gripper.  @Developed by Arastoo Khajehee 
    /// </summary>
    public class ActionRG6Gripper : Action
    {
        public int gripperDistance;
        public int gripStrength;
        public int waitTime;

        public override ActionType Type => ActionType.OnrobotRG6;

        public ActionRG6Gripper(int gripperValue, int heldObjectWeight, int waitTime) : base()
        {
            this.gripperDistance = gripperValue;
            this.gripStrength = heldObjectWeight;
            this.waitTime = waitTime;   
        }

        public override string ToString()
        {
            
                return string.Format("Set gripper distance to {0} mm with a {1}-newton power_limit pausing {2} milliseconds",
                    this.gripperDistance,
                    this.gripStrength,
                    this.waitTime);
        }


        public override string ToInstruction()
        {
            
            return string.Format("GripperTo({0},{1},{2});",
                this.gripperDistance,
                this.gripStrength,
                this.waitTime
            );

        }
    }
}