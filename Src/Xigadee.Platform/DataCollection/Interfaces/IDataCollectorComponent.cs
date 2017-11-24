namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by a data collection component that receives events and passes them on.
    /// </summary>
    /// <seealso cref="Xigadee.IRequireServiceOriginator" />
    public interface IDataCollectorComponent: IRequireServiceOriginator
    {
        bool IsSupported(DataCollectionSupport support);

        void Write(EventHolder eventData);

        bool CanFlush { get; }

        void Flush();
    }
}
