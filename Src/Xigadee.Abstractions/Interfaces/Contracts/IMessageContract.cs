namespace Xigadee
{
    /// <summary>
    /// This is the base interface used for message contracts.
    /// </summary>
    public interface IMessageContract
    {

    }
    /// <summary>
    /// This is the generic message contract.
    /// </summary>
    /// <typeparam name="P">The contract type.</typeparam>
    /// <seealso cref="Xigadee.IMessageContract" />
    public interface IMessageContractPayload<P>:IMessageContract
    {

    }
}
