using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// The attribute is used to prepopulate the entity generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PrepopulateEntitiesAttribute : Attribute
    {
        /// <summary>
        /// This constructor should be used to define the entity types.
        /// </summary>
        /// <param name="types"></param>
        public PrepopulateEntitiesAttribute(params Type[] types)
        {
            EntityTypes = types ?? new Type[] { };
        }

        /// <summary>
        /// This is the list of supported entity types.
        /// </summary>
        public Type[] EntityTypes { get; }
    }
}
