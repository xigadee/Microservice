namespace Xigadee
{
    /// <summary>
    /// This attribute can be applied to an entity reference. 
    /// A reference is a field can be owned by only one entity, i.e. key=username and value=user1. If other entities
    /// attempt to create the same reference they will be rejected.
    /// </summary>
    /// <seealso cref="Xigadee.EntityHintAttribute" />
    public class EntityReferenceHintAttribute : EntityHintAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityReferenceHintAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public EntityReferenceHintAttribute(string key) : base(key)
        {
        }
    }
}
