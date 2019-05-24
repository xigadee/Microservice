using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Xigadee
{
    /// <summary>
    /// This class is used to examine a context and extract the repositories that require creation at runtime.
    /// </summary>
    public class ContextDirectives
    {
        #region Declarations
        readonly IApiStartupContext _ctx;

        //Filter for the attribute types that we wish to get.
        readonly Func<CustomAttributeData, bool> attrFilterRepoRoot = (d) =>
            d.AttributeType == typeof(RepositoriesProcessAttribute) ||
            d.AttributeType == typeof(StopRepositoriesProcessAttribute)
            ;

        readonly Func<CustomAttributeData, bool> attrFilterRepoClass = (d) =>
            d.AttributeType == typeof(RepositoryLoadAttribute)
            //|| d.AttributeType == typeof(StopRepositoriesProcessAttribute)
            ;

        //Filter for the attribute types that we wish to get.
        readonly Func<CustomAttributeData, bool> attrFilterStartStop = (d) =>
            d.AttributeType == typeof(ModuleStartStopAttribute)
            ;

        //Filter for the attribute types that we wish to get.
        readonly Func<CustomAttributeData, bool> attrFilterSingleton = (d) =>
            d.AttributeType == typeof(RegisterAsSingletonAttribute) ||
            d.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)
            ;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="ctx">The api context.</param>
        public ContextDirectives(IApiStartupContext ctx)
        {
            _ctx = ctx;
        }
        #endregion

        #region GetAttributeData(Func<CustomAttributeData, bool> attrFilter)
        /// <summary>
        /// This method returns the specific attribute data for property and method declarations.
        /// </summary>
        /// <param name="attrFilter">The specific filter.</param>
        /// <returns>Returns the list of data.</returns>
        public IEnumerable<(CustomAttributeData[], MethodInfo)> GetAttributeData(Func<CustomAttributeData, bool> attrFilter)
            => GetAttributeData(_ctx.GetType(), attrFilter);
        #endregion
        #region GetAttributeData(Type oType, Func<CustomAttributeData, bool> attrFilter)
        /// <summary>
        /// This method returns the specific attribute data for property and method declarations.
        /// </summary>
        /// <param name="oType">The object type.</param>
        /// <param name="attrFilter">The specific filter.</param>
        /// <returns>Returns the list of data.</returns>
        public static IEnumerable<(CustomAttributeData[], MethodInfo)> GetAttributeData(Type oType, Func<CustomAttributeData, bool> attrFilter)
        {
            var resultM = oType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
                .Select((m) => (m.CustomAttributes.Where(attrFilter).ToArray(), m))
                ;

            var resultP = oType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
                .Select((m) => (m.CustomAttributes.Where(attrFilter).ToArray(), m.GetGetMethod()))
                ;

            return resultM.Union(resultP);
        }
        #endregion

        #region SingletonRegistrationsExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public IEnumerable<(Type sType, object service)> SingletonRegistrationsExtract()
        {
            //Filter for the attribute types that we wish to get.
            var results = GetAttributeData(attrFilterSingleton);

            foreach (var result in results)
            {
                var attrs = result.Item1.ToList();

                if (attrs.Contains((a) => a.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)))
                {
                    //TODO: OK, as we may have multiple attributes, with deny set for a specific registration type, 
                    //we need to filter out the ones registered and then adjust the collection.
                    continue;
                }

                //Is the stop attribute defined, if so skip.
                if (attrs.Count == 0)
                    continue;

                //Ok, extract the object and return the type. We may return multiple registrations for a single property.
                object item = result.Item2.Invoke(_ctx, new object[] { });

                if (item != null)
                    foreach (CustomAttributeData ad in attrs)
                    {
                        //OK, we know this is a SingletonRegistrationAttribute, so we just need to get the constructor parameter
                        if (ad.ConstructorArguments.Count == 0 || ad.ConstructorArguments[0].Value == null)
                            yield return (result.Item2.ReturnType, item);
                        else
                            yield return ((Type)ad.ConstructorArguments[0].Value, item);
                    }
            }

            yield break;
        }
        #endregion
        #region RepositoryProcessExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public RepositoryDirectiveCollection RepositoryProcessExtract()
        {
            var coll = new RepositoryDirectiveCollection();

            //Filter for the attribute types that we wish to get.
            var results = GetAttributeData(attrFilterRepoRoot);

            foreach (var result in results)
            {
                var attrs = result.Item1.ToList();

                //OK, if there is a stop defined then ignore this module
                if (attrs.Contains((a) => a.AttributeType == typeof(StopRepositoriesProcessAttribute)))
                    continue;

                //Ok, extract the object and we will scan that for the deeper attributes.
                //We need to extract as we have to scan the actual object.
                object item = result.Item2.Invoke(_ctx, new object[] { });

                var repos = item.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where((m) => m.CustomAttributes.Contains((a) => attrFilterRepoClass(a)))
                    .Select((pInfo) => (pInfo.CustomAttributes.Where(attrFilterRepoClass).First(), pInfo))
                    .ToList()
                    ;

                if (repos.Count == 0)
                    continue;

                var mod = new RepositoryDirectiveModule() { Module = item };
                coll.Modules.Add(mod);

                //OK, let's process each repo directive.
                foreach (var repoDirective in repos)
                {
                    var d = new RepositoryDirective(item, repoDirective.pInfo);
                    mod.Directives.Add(d);
                }
            }

            return coll;
        }
        #endregion

        #region ModuleStartStopExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public IEnumerable<IApiModuleService> ModuleStartStopExtract()
        {
            //Filter for the attribute types that we wish to get.
            var results = GetAttributeData(attrFilterStartStop);

            foreach (var result in results)
            {
                var mi = result.Item2;
                var obj = mi.Invoke(_ctx, new object[] { });

                if (obj is IApiModuleService)
                    yield return obj as IApiModuleService;
            }

            yield break;
        }
        #endregion
    }
}