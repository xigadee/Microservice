using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify the supported features for a controller.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class EntityControllerFeaturesSupportAttribute:Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityControllerFeatureSupportAttribute"/> class.
        /// </summary>
        /// <param name="features">The supported features.</param>
        public EntityControllerFeaturesSupportAttribute(EntityControllerFeatures features)
        {
            Features = features;
        }

        public EntityControllerFeatures Features { get; }
    }
}
