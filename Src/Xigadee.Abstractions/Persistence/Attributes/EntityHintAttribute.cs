using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to mark a property as a reference or property of an entity.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class EntityHintAttribute:Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityHintAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        internal EntityHintAttribute(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; }
    }
}
