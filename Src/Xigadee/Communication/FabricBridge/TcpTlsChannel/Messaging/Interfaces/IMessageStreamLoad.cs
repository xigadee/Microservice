namespace Xigadee
{
    /// <summary>
    /// This interface is used to load the message with its initial data.
    /// </summary>
    public interface IMessageStreamLoad : 
        IMessageLoad, IMessageLoadInitialize, IMessageLoadData
    {
    }
}
