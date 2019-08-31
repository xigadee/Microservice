namespace Xigadee
{
    /// <summary>
    /// This enumeration determines the termination style for the fragment.
    /// </summary>
    public enum FragmentTerminationType
    {
        /// <summary>
        /// The fragment has a fixed number of bytes.
        /// </summary>
        ByteLength,
        /// <summary>
        /// The fragment will terminate when the byte array end matches the termination array.
        /// </summary>
        Terminator,
        /// <summary>
        /// The fragment will terminate when the byte array end matches the delimiter structure.
        /// </summary>
        Custom
    }
}
