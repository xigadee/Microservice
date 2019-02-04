namespace Xigadee
{
    /// <summary>
    /// This class is used to signal statistics events.
    /// </summary>
    public class StatisticsEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// The new statistics.
        /// </summary>
        public Microservice.Statistics Statistics { get; set; }
    }
}
