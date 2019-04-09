using System;
using System.Diagnostics;
using System.Reflection;
namespace Xigadee
{
    /// <summary>
    /// This is the root directive class that holds the reference to the actual property that needs to be set.
    /// </summary>
    [DebuggerDisplay("IRepositoryAsync<{TypeKey.Name},{TypeEntity.Name}>")]
    public class RepositoryDirective
    {
        #region Constructor
        /// <summary>
        /// This is the core 
        /// </summary>
        /// <param name="module">This is the module that requires the repository to be set.</param>
        /// <param name="pInfo">This is the property info for the repository to be set.</param>
        public RepositoryDirective(object module, PropertyInfo pInfo)
        {
            Module = module;
            Property = pInfo;

            if (!(pInfo.CanWrite || pInfo.CanRead))
                throw new ArgumentOutOfRangeException($"{nameof(RepositoryDirective)}: Property '{pInfo.Name}' Cannot be read or written to.");

            var returnType = pInfo.GetGetMethod().ReturnType;

            //Let's just check that the return type is a repository.
            if (!returnType.IsSubclassOfRawGeneric(typeof(IRepositoryAsync<,>)))
                throw new ArgumentOutOfRangeException($"{nameof(RepositoryDirective)}: '{returnType.Name}' does not implement IRepositoryAsync<,>");

            TypeKey = returnType.GenericTypeArguments[0];
            TypeEntity = returnType.GenericTypeArguments[1];

            RepositoryType = typeof(IRepositoryAsync<,>).MakeGenericType(TypeKey, TypeEntity);
        }
        #endregion

        /// <summary>
        /// This is the module to be set.
        /// </summary>
        public object Module { get; }
        /// <summary>
        /// THis is the specific property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// This is the key type,
        /// </summary>
        public Type TypeKey { get; }
        /// <summary>
        /// This is the entity type.
        /// </summary>
        public Type TypeEntity { get; }

        /// <summary>
        /// This is the generic repository type, IRepositoryAsync
        /// </summary>
        public Type RepositoryType { get; }

        /// <summary>
        /// This gets the repository.
        /// </summary>
        /// <returns></returns>
        public object Get()
        {
            return Property.GetMethod.Invoke(Module, new object[] { });
        }

        /// <summary>
        /// This method sets the repository.
        /// </summary>
        /// <param name="repository">The repository to set.</param>
        public void Set(object repository)
        {
            Property.SetMethod.Invoke(Module, new object[] { repository });
        }
    }
}
