using System;

namespace Machina
{
    /// <summary>
    /// A no-op action that participates in the normal Machina queue and execution flow.
    /// </summary>
    public class ActionSyncCurrent : Action
    {
        public override ActionType Type => ActionType.SyncCurrent;

        public ActionSyncCurrent() : base()
        {
        }

        public override string ToString()
        {
            return "Sync current robot state marker";
        }

        public override string ToInstruction()
        {
            return "SyncCurrent();";
        }
    }
}
