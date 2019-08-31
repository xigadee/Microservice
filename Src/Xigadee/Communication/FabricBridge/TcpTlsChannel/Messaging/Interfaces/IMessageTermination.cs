namespace Xigadee
{
    public interface IMessageTermination
    {
        /// <summary>
        /// This property specifies whether the fragment byte array has reached the termination requirements
        /// </summary>
        bool IsTerminator { get; }

        int CarryOver { get; }

        bool Match(byte[] buffer, int offset, int count, out int length, out long? bodyLength);

    }
}
