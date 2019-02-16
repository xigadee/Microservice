using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to annotate a shortcut for a key for a particular group.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigSettingKeyAttribute:Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettingKeyAttribute"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        public ConfigSettingKeyAttribute(string category = null, string description = null)
        {
            Category = category;
            Description = description;
        }
        /// <summary>
        /// Gets the category for the key used to group them together.
        /// </summary>
        public string Category { get; }
        /// <summary>
        /// Gets the description for the key.
        /// </summary>
        public string Description { get; }
    }

    /// <summary>
    /// This attribute is used to identify a specific method that is connected to a shortcut key.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class ConfigSettingAttribute: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSettingAttribute"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        public ConfigSettingAttribute(string category, string description = null)
        {
            Category = category;
            Description = description;
        }
        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; }
        /// <summary>
        /// Gets the description for the key.
        /// </summary>
        public string Description { get; }
    }
}
