namespace Xigadee
{
    /// <summary>
    /// This interface is used to present outgoing message functionality to command logic.
    /// </summary>
    /// <seealso cref="Xigadee.ICommandInitiator" />
    /// <seealso cref="Xigadee.IMicroserviceDispatch" />
    public interface ICommandOutgoing: ICommandInitiator, IMicroserviceDispatch
    {
    }
}
