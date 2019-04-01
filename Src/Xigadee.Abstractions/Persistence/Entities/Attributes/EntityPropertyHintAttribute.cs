namespace Xigadee
{
    /// <summary>
    /// This attribute can be applied to an entity property. 
    /// A property is a field can be owned by many entities and is used for searching, i.e. city=London, name=Paul, etc.
    /// where the key value is 'city', 'name' etc.
    /// </summary>
    /// <seealso cref="Xigadee.EntityHintAttribute" />
    public class EntityPropertyHintAttribute : EntityHintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyHintAttribute"/> class.
        /// </summary>
        /// <param name="key">The property key.</param>
        public EntityPropertyHintAttribute(string key) : base(key)
        {
        }
    }
}
