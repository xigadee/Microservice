using System.ComponentModel;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by objects that support the messaging format.
    /// </summary>
    public interface IMessage : ISupportInitialize, IMessageStreamLoad
    {
        bool SupportsInitialization { get; }

        bool CanRead { get; }
        bool CanWrite { get; }

        long Position { get;set; }
        long Length { get; }
        long? BodyLength { get; }

        int Read(byte[] buffer, int offset, int count);
        int ReadByte();

        int Write(byte[] buffer, int offset, int count);
        int WriteByte(byte value);

        /// <summary>
        /// This property indicates the message direction.
        /// </summary>
        MessageDirection Direction { get;}

        /// <summary>
        /// This property indicates whether this section signals the end of the message.
        /// </summary>
        bool IsTerminator { get; }

        string DebugString { get; }

        byte[] ToArray();
        byte[] ToArray(bool copy);
    }
}
