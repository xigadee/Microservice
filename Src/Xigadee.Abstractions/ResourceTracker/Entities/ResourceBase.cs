using System;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used by resource objects.
    /// </summary>
    public abstract class ResourceBase: IResourceBase
    {
        /// <summary>
        /// The unique id of the resource.
        /// </summary>
        public Guid ResourceId { get; } = Guid.NewGuid();
        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name { get; protected set;}
    }
}