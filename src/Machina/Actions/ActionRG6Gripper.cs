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
        public GripperType gripperType;
        public double gripperValue;
        public double heldObjectWeight;
        public GripperRunStop gripperRunStop;
        public bool relative;

        public override ActionType Type => ActionType.OnrobotRG6;

        public ActionRG6Gripper(GripperType gripperType, double gripperValue, double heldObjectWeight, GripperRunStop gripperRunStop, bool relative) : base()
        {
            this.gripperType = gripperType;
            this.gripperValue = gripperValue;
            this.heldObjectWeight = heldObjectWeight;
            this.gripperRunStop = gripperRunStop;
            this.relative = relative;
        }

        public override string ToString()
        {
            if (relative)
            {
                return string.Format("{0} grip distance by {1} mm {2} with {3} kilograms",
                    this.gripperValue < 0 ? "Decrease" : "Increase",
                    this.gripperValue,
                    this.gripperRunStop == GripperRunStop.Inplace ? "inplace" : "while moving",
                    this.heldObjectWeight);
            }
            else
            {
                return string.Format("Set gripper distance to {0} mm {1} with {2} kilograms",
                    this.gripperValue,
                    this.gripperRunStop == GripperRunStop.Inplace ? "inplace" : "while moving",
                    this.heldObjectWeight);
            }
        }


        public override string ToInstruction()
        {
            if (this.relative)
            {
                return string.Format("Gripper({0},{1},{2});",
                    this.gripperValue,
                    this.heldObjectWeight,
                    (Enum.GetName(typeof(GripperRunStop), this.gripperRunStop))
                );
            }
            else
            {
                return string.Format("GripperTo({0},{1},{2});",
                    this.gripperValue,
                    this.heldObjectWeight,
                    (Enum.GetName(typeof(GripperRunStop), this.gripperRunStop))
                );

            }
        }
    }
}