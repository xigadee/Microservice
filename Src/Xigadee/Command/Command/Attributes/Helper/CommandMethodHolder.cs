namespace Xigadee
{
    /// <summary>
    /// This holder contains the reference between a method signature and it's associated attribute.
    /// </summary>
    /// <typeparam name="A">The attribute type.</typeparam>
    /// <typeparam name="S">The signature type.</typeparam>
    public class CommandMethodHolder<A,S>
        where A : CommandMethodAttributeBase
        where S : CommandSignatureBase
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        public A Attribute { get; set; }
        /// <summary>
        /// Gets the signature.
        /// </summary>
        public S Signature { get; set;}
        /// <summary>
        /// Gets the associated reference between the two items.
        /// </summary>
        public string Reference { get; set;}
    }
}
