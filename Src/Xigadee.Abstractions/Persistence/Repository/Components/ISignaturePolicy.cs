namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by a class that calculates and verifies the signature of an entity.
    /// </summary>
    public interface ISignaturePolicy
    {
        /// <summary>
        /// This method returns a case-sensitive string that forms a unique signature for an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string Calculate(object entity);
        /// <summary>
        /// This method returns true if the entity and signature passed are correct.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <param name="signature">The signature to verify for the entity.</param>
        /// <returns>Returns true if the signature is verified.</returns>
        bool Verify(object entity, string signature);
    }
}