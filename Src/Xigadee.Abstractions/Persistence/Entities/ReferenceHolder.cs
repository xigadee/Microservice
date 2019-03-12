namespace Xigadee
{
    /// <summary>
    /// This class holds the references for the entity
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    public class ReferenceHolder<K>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public K Key { get; set; } = default(K);
        /// <summary>
        /// Gets or sets the type of the reference.
        /// </summary>
        public string RefType { get; set; }
        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public string RefValue { get; set; }
    }
}
