namespace Xigadee
{
    /// <summary>
    /// This attribute can be applied to an entity property. 
    /// A property is a field can be owned by many entities and is used for searching, i.e. city=London, name=Paul, etc.
    /// where the key value is 'city', 'name' etc.
    /// </summary>
    /// <seealso cref="Xigadee.EntityHintAttribute" />
    public class EntityVersionHintAttribute : EntityHintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityVersionHintAttribute"/> class.
        /// </summary>
        public EntityVersionHintAttribute() : base(null)
        {
        }
    }
}
