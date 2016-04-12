namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that integrate with the Microservice process loop.
    /// </summary>
    public interface ITaskManagerProcess
    {
        bool CanProcess();

        void Process(TaskManagerAvailability availability);
    }
}