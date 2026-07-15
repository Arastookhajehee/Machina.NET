using System;

namespace Machina
{
    /// <summary>
    /// A no-op action that participates in the normal Machina queue and execution flow.
    /// </summary>
    public class ActionSyncCurrent : Action
    {
        public bool flushPending;
        public override ActionType Type => ActionType.SyncCurrent;

        public ActionSyncCurrent(bool flushPending = false) : base()
        {
            this.flushPending = flushPending;
        }

        public override string ToString()
        {
            return $"Sync current robot state{(flushPending ? " and flush pending actions" : " without flushing pending actions")}";
        }

        public override string ToInstruction()
        {
            return $"SyncCurrent({this.flushPending.ToString().ToLowerInvariant()});";
        }
    }
}
