namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by the Pipeline.
    /// </summary>
    public interface IPipeline: IPipelineBase
    {
        /// <summary>
        /// This is the microservice.
        /// </summary>
        IMicroservice Service { get; }

        /// <summary>
        /// This is the microservice configuration.
        /// </summary>
        IEnvironmentConfiguration Configuration { get; }
    }
}
