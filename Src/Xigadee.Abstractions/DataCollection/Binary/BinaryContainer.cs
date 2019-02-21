namespace Xigadee
{
    /// <summary>
    /// This class holds the binary container.
    /// </summary>
    public class BinaryContainer
    {
        /// <summary>
        /// This is the binary blob.
        /// </summary>
        public byte[] Blob { get; set; }
        /// <summary>
        /// This is the content type of the entity.
        /// </summary>
        public string ContentType { get; set; } = "application/json; charset=utf-8";
        /// <summary>
        /// This is the encoding type of the entity.
        /// </summary>
        public string ContentEncoding { get; set; } = null;
    }
}
