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
        public double gripperDistance;
        public double gripStrength;

        public override ActionType Type => ActionType.OnrobotRG6;

        public ActionRG6Gripper(int gripperValue, int heldObjectWeight) : base()
        {
            this.gripperDistance = gripperValue;
            this.gripStrength = heldObjectWeight;
        }

        public override string ToString()
        {

            return string.Format("Set gripper distance to {0} mm with a {1}-newton force",
                this.gripperDistance,
                this.gripStrength
                );
        }


        public override string ToInstruction()
        {
            
            return string.Format("GripperTo({0},{1});",
                this.gripperDistance,
                this.gripStrength
            );

        }
    }
}