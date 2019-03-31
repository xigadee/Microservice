using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class is used to return a resolver for entities that support hints.
    /// </summary>
    public static class EntityHintHelper
    {
        static ConcurrentDictionary<Type, EntityHintResolver> _resolverCache = new ConcurrentDictionary<Type, EntityHintResolver>();

        /// <summary>
        /// Returns the resolver for the specified entity type.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the resolver</returns>
        public static EntityHintResolver Resolve<E>(E entity)
        {
            return Resolve(typeof(E));
        }
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the resolver</returns>
        public static EntityHintResolver Resolve(Type type)
        {
            return _resolverCache.GetOrAdd(type, (t) => new EntityHintResolver(t));
        }
    }

    /// <summary>
    /// This class is used to analyse the entity and provide a set of properties and references.
    /// </summary>
    public class EntityHintResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityHintResolver"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public EntityHintResolver(Type type)
        {
            EntityType = type;



            var rs = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityPropertyHintAttribute>(), (m, a) => ((EntityPropertyHintAttribute)a, m))
                //.Where((o) => inherit || o.Item2.DeclaringType == objectType)
                .ToList()
                ;

            var rs2 = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityPropertyHintAttribute>(), (m, a) => ((EntityPropertyHintAttribute)a, m))
                //.Where((o) => inherit || o.Item2.DeclaringType == objectType)
                .ToList()
                ;

        }

        protected MethodInfo InfoId { get; set; }

        protected MethodInfo InfoVersion { get; set; }

        protected List<MethodInfo> InfoProperties { get; set; }

        protected List<MethodInfo> InfoReferences { get; set; }

        ///// <summary>
        ///// This static helper returns the list of methods that are decorated with the attribute type.
        ///// </summary>
        //public static IEnumerable<(A attr, MethodInfo method)> CommandMethods<A>(this Type objectType, bool inherit)
        //    where A : Attribute
        //{
        //    return objectType
        //        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        //        .SelectMany((m) => m.GetCustomAttributes<A>(), (m, a) => ((A)a, m))
        //        .Where((o) => inherit || o.Item2.DeclaringType == objectType)
        //        ;
        //}

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; }

        public IEnumerable<KeyValuePair<string, string>> References(object item)
        {
            yield break;
        }

        public IEnumerable<KeyValuePair<string, string>> Properties(object item)
        {
            yield break;
        }
    }
}
