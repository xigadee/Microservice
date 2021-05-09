using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This class is used to examine a context and extract the repositories that require creation at runtime.
    /// </summary>
    public class ContextDirectives
    {
        #region Declarations
        readonly IApiStartupContextBase _ctx;

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
        public ContextDirectives(IApiStartupContextBase ctx)
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
        public IEnumerable<(CustomAttributeData[] attr , MethodInfo mi)> GetAttributeData(Func<CustomAttributeData, bool> attrFilter)
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

        #region ModulesCreate()
        /// <summary>
        /// This metod will create and set any module set for automatic creation.
        /// </summary>
        public void ModulesCreate()
        {
            foreach (var mi in ModuleStartStopExtractMethodInfo(ModuleStartStopMode.Create))
            {
                var serv = ContextDirectives.miInvoke(this._ctx, mi);
                if (serv == null)
                {
                    var set = mi.DeclaringType.GetProperties().FirstOrDefault(m => m.GetMethod == mi);
                    var setmi = set?.SetMethod;
                    var takesArg = mi.GetParameters().Length == 1;
                    var noReturn = mi.ReturnType == typeof(void);

                    //OK, we need to create the module here.
                    var mt = mi.ReturnType;
                    var parentRestrictions = mt.IsAbstract || !mt.IsClass;

                    if (parentRestrictions)
                        throw new ArgumentOutOfRangeException($"{nameof(ModulesCreate)}: Cannot create module: {mt.Name} is abstract or is not a class.");

                    serv = ServiceHarnessHelper.DefaultCreator(mt)() as IApiModuleService;

                    setmi.Invoke(_ctx, new object[] { serv });
                }
            }
        }
        #endregion
        #region ModulesLoad()
        /// <summary>
        /// This method will connect any module set for automatic connection to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="lf">The logging factory.</param>
        public void ModulesLoad()
        {
            foreach (var mi in ModuleStartStopExtractMethodInfo(ModuleStartStopMode.Load))
            {
                var serv = ContextDirectives.miInvoke(_ctx, mi);
                serv.Load(_ctx);
            }
        }
        #endregion

        #region ModulesConnect(ILoggerFactory lf)
        /// <summary>
        /// This method will connect any module set for automatic connection to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="lf">The logging factory.</param>
        public void ModulesConnect(ILoggerFactory lf)
        {
            foreach (var mi in ModuleStartStopExtractMethodInfo(ModuleStartStopMode.Connect))
            {
                var serv = ContextDirectives.miInvoke(_ctx, mi);
                serv.Connect(_ctx, lf.CreateLogger(mi.ReturnType));
            }
        }
        #endregion

        #region ModulesStart(CancellationToken cancellationToken)
        /// <summary>
        /// This module will start any module marked for automatic start.
        /// </summary>
        public Task ModulesStart(CancellationToken cancellationToken) => Task.WhenAll(ModuleStartStopExtract(ModuleStartStopMode.Start).Select(m => m.Start(cancellationToken))); 
        #endregion
        #region ModulesStop(CancellationToken cancellationToken)
        /// <summary>
        /// This method will stop any module set to stop automatically.
        /// </summary>
        /// <returns></returns>
        public Task ModulesStop(CancellationToken cancellationToken) => Task.WhenAll(ModuleStartStopExtract(ModuleStartStopMode.Stop).Select(m => m.Stop(cancellationToken))); 
        #endregion

        #region ModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
        /// <summary>
        /// This method examines the context and extracts any start/stop declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        private IEnumerable<MethodInfo> ModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
        {
            //Filter for the attribute types that we wish to get.
            var results = GetAttributeData(attrFilterStartStop);

            foreach (var result in results)
            {
                if (result.attr.Length == 0)
                    continue;

                var attrMode = ModuleStartStopAttributeModeExtract(result.attr[0]);
                if (!attrMode.HasValue || (attrMode.Value & mode) == 0)
                    continue;

                yield return result.mi;
            }

            yield break;
        }
        private static ModuleStartStopMode? ModuleStartStopAttributeModeExtract(CustomAttributeData data)
        {
            if (data.AttributeType != typeof(ModuleStartStopAttribute))
                return null;

            if (data.ConstructorArguments.Count != 1)
                return null;

            var arg = data.ConstructorArguments[0];
            if (arg.ArgumentType != typeof(ModuleStartStopMode))
                return null;

            return (ModuleStartStopMode)arg.Value;
        }
        #endregion
        #region ModuleStartStopExtract(ModuleStartStopMode mode)
        /// <summary>
        /// This method examines the context and extracts any start/stop declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        private IEnumerable<IApiModuleService> ModuleStartStopExtract(ModuleStartStopMode mode)
        {
            foreach (var mi in ModuleStartStopExtractMethodInfo(mode))
            {
                var obj = miInvoke(_ctx, mi);

                if (obj != null)
                    yield return obj;
            }

            yield break;
        }

        /// <summary>
        /// This function extracts a module from it's property definition.
        /// </summary>
        public static Func<object, MethodInfo, IApiModuleService> miInvoke = (parent, mi) =>
        {
            var obj = mi.Invoke(parent, new object[] { });

            if (obj is IApiModuleService)
                return obj as IApiModuleService;

            return null;
        };

        /// <summary>
        /// This function extracts a module from it's property definition.
        /// </summary>
        public static Func<object, MethodInfo, IApiModuleService> miInvokeCreate = (parent, mi) =>
        {
            var obj = mi.Invoke(parent, new object[] { });

            if (obj is IApiModuleService)
                return obj as IApiModuleService;

            //OK we need to create the module using the default constructor.
            var rt = mi.ReturnType;



            return null;
        };

        #endregion
    }
}