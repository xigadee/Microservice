namespace Xigadee
{
    public partial class CommandHarness<C, S, P>
    {
        /// <summary>
        /// This method manually triggers the outgoing request timeout function.
        /// </summary>
        public void OutgoingTimeoutScheduleExecute()
        {
            ScheduleExecute(true, (s) => s.ScheduleType == "CommandTimeoutSchedule" && ((CommandTimeoutSchedule)s).CommandId == Service.ComponentId);
        }
    }
}
