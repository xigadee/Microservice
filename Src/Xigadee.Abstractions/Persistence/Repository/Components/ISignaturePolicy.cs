namespace Xigadee
{
    public interface ISignaturePolicy
    {
        string Calculate(object entity);
        bool Verify(object entity, string signature);
    }
}