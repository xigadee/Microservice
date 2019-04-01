using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityHintResolver"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public EntityHintResolver(Type type)
        {
            EntityType = type;

            ReflectionCalculate();
        } 
        #endregion

        #region ReflectionCalculate()
        /// <summary>
        /// Populates the entity reflection parameters.
        /// </summary>
        protected virtual void ReflectionCalculate()
        {
            MethodInfoProperties = EntityType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityPropertyHintAttribute>(), (m, a) => ((EntityPropertyHintAttribute)a, (MethodInfo)m))
                .Union(EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .SelectMany((m) => m.GetCustomAttributes<EntityPropertyHintAttribute>(), (m, a) => ((EntityPropertyHintAttribute)a, (MethodInfo)m.GetMethod))
                ).ToList();

            MethodInfoReferences = EntityType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityReferenceHintAttribute>(), (m, a) => ((EntityReferenceHintAttribute)a, (MethodInfo)m))
                .Union(EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .SelectMany((m) => m.GetCustomAttributes<EntityReferenceHintAttribute>(), (m, a) => ((EntityReferenceHintAttribute)a, (MethodInfo)m.GetMethod))
                ).ToList();

            MethodInfoId = EntityType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityIdHintAttribute>(), (mt, a) => mt)
                .Union(EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .SelectMany((m) => m.GetCustomAttributes<EntityIdHintAttribute>(), (mt, a) => mt.GetMethod)
                ).FirstOrDefault();

            MethodInfoVersion = EntityType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany((m) => m.GetCustomAttributes<EntityVersionHintAttribute>(), (mt, a) => mt)
                .Union(EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .SelectMany((m) => m.GetCustomAttributes<EntityVersionHintAttribute>(), (mt, a) => mt.GetMethod)
                ).FirstOrDefault();
        }
        #endregion

        #region EntityType
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        public Type EntityType { get; } 
        #endregion

        #region Id...
        /// <summary>
        /// Gets or sets the information identifier method info.
        /// </summary>
        protected MethodInfo MethodInfoId { get; set; }
        /// <summary>
        /// Gets a value indicating whether Id is supported.
        /// </summary>
        public bool SupportsId => MethodInfoId != null;
        /// <summary>
        /// Identifiers the specified entity.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public K Id<K>(object entity)
        {
            if (MethodInfoId == null)
                return default(K);

            return (K)MethodInfoId.Invoke(entity, EmptyObjects);
        }
        #endregion
        #region Version...
        /// <summary>
        /// Gets or sets the information version.
        /// </summary>
        protected MethodInfo MethodInfoVersion { get; set; }
        /// <summary>
        /// Gets a value indicating whether Version is supported.
        /// </summary>
        public bool SupportsVersion => MethodInfoVersion != null;
        /// <summary>
        /// Versions the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Cannot convert version parameter to string.</exception>
        public string Version(object entity)
        {
            if (!SupportsVersion)
                return null;

            var v = MethodInfoVersion.Invoke(entity, EmptyObjects);

            if (v is string)
                return (string)v;

            //Special case for GUID
            if (MethodInfoVersion.ReturnType == typeof(Guid))
                return ((Guid)v).ToString("N").ToUpperInvariant();

            return ConvertValueToString(v, MethodInfoVersion);
        }
        #endregion
        #region References ...
        /// <summary>
        /// Gets or sets the MethodInfo references.
        /// </summary>
        protected List<(EntityReferenceHintAttribute, MethodInfo)> MethodInfoReferences { get; set; }
        /// <summary>
        /// Gets a value indicating whether references are supported.
        /// </summary>
        public bool SupportsReferences => (MethodInfoReferences?.Count ?? 0) > 0;

        /// <summary>
        /// Returns the entity references for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An enumerable list of key value pairs.</returns>
        public IEnumerable<KeyValuePair<string, string>> References(object entity)
        {
            foreach (var info in MethodInfoReferences)
            {
                var v = ConvertToString(entity, info.Item2);
                yield return new KeyValuePair<string, string>(info.Item1.Key, v);
            }
        }
        #endregion
        #region Properties...
        /// <summary>
        /// Gets or sets the MethodInfo properties.
        /// </summary>
        protected List<(EntityPropertyHintAttribute, MethodInfo)> MethodInfoProperties { get; set; }
        /// <summary>
        /// Gets a value indicating whether properties are supported.
        /// </summary>
        public bool SupportsProperties => (MethodInfoProperties?.Count ?? 0) > 0;
        /// <summary>
        /// Returns the entity properties for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An enumerable list of key value pairs.</returns>
        public IEnumerable<KeyValuePair<string, string>> Properties(object entity)
        {
            foreach (var info in MethodInfoProperties)
            {
                var v = ConvertToString(entity, info.Item2);
                yield return new KeyValuePair<string, string>(info.Item1.Key, v);
            }
        } 
        #endregion

        #region EmptyObjects
        /// <summary>
        /// Gets the empty objects helper declaration.
        /// </summary>
        object[] EmptyObjects { get; } = new object[] { }; 
        #endregion
        #region ConvertToString(object entity, MethodInfo p)
        /// <summary>
        /// Resolves the value and converts to a string.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="p">The method info.</param>
        /// <returns></returns>
        protected string ConvertToString(object entity, MethodInfo p)
        {
            var v = p.Invoke(entity, EmptyObjects);

            return ConvertValueToString(v, p);
        }
        #endregion
        #region ConvertValueToString(object v, MethodInfo p)
        /// <summary>
        /// Converts to a string using the in-built type converters.
        /// </summary>
        /// <param name="v">The value.</param>
        /// <param name="p">The method info.</param>
        /// <returns>Returns a string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Cannot convert version parameter {MethodInfoVersion.ReturnType.Name}</exception>
        protected string ConvertValueToString(object v, MethodInfo p)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(p.ReturnType);

            if (converter.CanConvertTo(typeof(string)))
                return converter.ConvertToString(v);

            throw new ArgumentOutOfRangeException($"{nameof(EntityHintResolver)} cannot convert version parameter {MethodInfoVersion.ReturnType.Name} to string.");
        } 
        #endregion
    }
}
