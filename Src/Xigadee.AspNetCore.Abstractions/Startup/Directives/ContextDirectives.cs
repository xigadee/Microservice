//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;

//namespace Xigadee
//{
//    /// <summary>
//    /// This class is used to examine a context and extract the repositories that require creation at runtime.
//    /// </summary>
//    public class ContextDirectives
//    {
//        #region Declarations
//        readonly IApiStartupContextBase _ctx;

//        //Filter for the attribute types that we wish to get.
//        readonly Func<CustomAttributeData, bool> attrFilterRepoRoot = (d) =>
//            d.AttributeType == typeof(RepositoriesProcessAttribute) ||
//            d.AttributeType == typeof(StopRepositoriesProcessAttribute)
//            ;

//        readonly Func<CustomAttributeData, bool> attrFilterRepoClass = (d) =>
//            d.AttributeType == typeof(RepositoryLoadAttribute)
//            //|| d.AttributeType == typeof(StopRepositoriesProcessAttribute)
//            ;

//        //Filter for the attribute types that we wish to get.
//        readonly Func<CustomAttributeData, bool> attrFilterStartStop = (d) =>
//            d.AttributeType == typeof(ModuleStartStopAttribute)
//            ;

//        //Filter for the attribute types that we wish to get.
//        readonly Func<CustomAttributeData, bool> attrFilterSingleton = (d) =>
//            d.AttributeType == typeof(RegisterAsSingletonAttribute) ||
//            d.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)
//            ;
//        #endregion
//        #region Constructor
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="ctx">The api context.</param>
//        public ContextDirectives(IApiStartupContextBase ctx)
//        {
//            _ctx = ctx;
//        }
//        #endregion

//        #region AttributeDataGet(Func<CustomAttributeData, bool> attrFilter)
//        /// <summary>
//        /// This method returns the specific attribute data for property and method declarations.
//        /// </summary>
//        /// <param name="attrFilter">The specific filter.</param>
//        /// <returns>Returns the list of data.</returns>
//        public IEnumerable<(CustomAttributeData[] attr , MethodInfo mi)> AttributeDataGet(Func<CustomAttributeData, bool> attrFilter)
//            => AttributeDataGet(_ctx.GetType(), attrFilter);
//        #endregion
//        #region AttributeDataGet(Type oType, Func<CustomAttributeData, bool> attrFilter)
//        /// <summary>
//        /// This method returns the specific attribute data for property and method declarations.
//        /// </summary>
//        /// <param name="oType">The object type.</param>
//        /// <param name="attrFilter">The specific filter.</param>
//        /// <returns>Returns the list of data.</returns>
//        public static IEnumerable<(CustomAttributeData[], MethodInfo)> AttributeDataGet(Type oType, Func<CustomAttributeData, bool> attrFilter)
//        {
//            var resultM = oType
//                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
//                .Select((m) => (m.CustomAttributes.Where(attrFilter).ToArray(), m))
//                ;

//            var resultP = oType
//                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
//                .Select((m) => (m.CustomAttributes.Where(attrFilter).ToArray(), m.GetGetMethod()))
//                ;

//            return resultM.Union(resultP);
//        }
//        #endregion

//        #region AttributeSingletonRegistrationsExtract()
//        /// <summary>
//        /// This method examines the context and extracts any singleton declarations.
//        /// </summary>
//        /// <returns>Returns the list of declarations.</returns>
//        public IEnumerable<(Type sType, object service)> AttributeSingletonRegistrationsExtract()
//        {
//            //Filter for the attribute types that we wish to get.
//            var results = AttributeDataGet(attrFilterSingleton);

//            foreach (var result in results)
//            {
//                var attrs = result.Item1.ToList();

//                if (attrs.Contains((a) => a.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)))
//                {
//                    //TODO: OK, as we may have multiple attributes, with deny set for a specific registration type, 
//                    //we need to filter out the ones registered and then adjust the collection.
//                    continue;
//                }

//                //Is the stop attribute defined, if so skip.
//                if (attrs.Count == 0)
//                    continue;

//                //Ok, extract the object and return the type. We may return multiple registrations for a single property.
//                object item = result.Item2.Invoke(_ctx, new object[] { });

//                if (item != null)
//                    foreach (CustomAttributeData ad in attrs)
//                    {
//                        //OK, we know this is a SingletonRegistrationAttribute, so we just need to get the constructor parameter
//                        if (ad.ConstructorArguments.Count == 0 || ad.ConstructorArguments[0].Value == null)
//                            yield return (result.Item2.ReturnType, item);
//                        else
//                            yield return ((Type)ad.ConstructorArguments[0].Value, item);
//                    }
//            }

//            yield break;
//        }
//        #endregion
//        #region AttributeRepositoryProcessExtract()
//        /// <summary>
//        /// This method examines the context and extracts any singleton declarations.
//        /// </summary>
//        /// <returns>Returns the list of declarations.</returns>
//        public RepositoryDirectiveCollection AttributeRepositoryProcessExtract()
//        {
//            var coll = new RepositoryDirectiveCollection();

//            //Filter for the attribute types that we wish to get.
//            var results = AttributeDataGet(attrFilterRepoRoot);

//            foreach (var result in results)
//            {
//                var attrs = result.Item1.ToList();

//                //OK, if there is a stop defined then ignore this module
//                if (attrs.Contains((a) => a.AttributeType == typeof(StopRepositoriesProcessAttribute)))
//                    continue;

//                //Ok, extract the object and we will scan that for the deeper attributes.
//                //We need to extract as we have to scan the actual object.
//                object item = result.Item2.Invoke(_ctx, new object[] { });

//                var repos = item.GetType()
//                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                    .Where((m) => m.CustomAttributes.Contains((a) => attrFilterRepoClass(a)))
//                    .Select((pInfo) => (pInfo.CustomAttributes.Where(attrFilterRepoClass).First(), pInfo))
//                    .ToList()
//                    ;

//                if (repos.Count == 0)
//                    continue;

//                var mod = new RepositoryDirectiveModule() { Module = item };
//                coll.Modules.Add(mod);

//                //OK, let's process each repo directive.
//                foreach (var repoDirective in repos)
//                {
//                    var d = new RepositoryDirective(item, repoDirective.pInfo);
//                    mod.Directives.Add(d);
//                }
//            }

//            return coll;
//        }
//        #endregion

//        #region AttributeModulesCreate()
//        /// <summary>
//        /// This metod will create and set any module set for automatic creation.
//        /// </summary>
//        public void AttributeModulesCreate()
//        {
//            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.Create))
//            {
//                var serv = MethodInfoInvokeContext(mi);

//                if (serv == null)
//                {
//                    try
//                    {
//                        var set = mi.DeclaringType.GetProperties().FirstOrDefault(m => m.GetMethod == mi);
//                        var setmi = set?.SetMethod;
//                        var takesArg = mi.GetParameters().Length == 1;
//                        var noReturn = mi.ReturnType == typeof(void);

//                        //OK, we need to create the module here.
//                        var mt = mi.ReturnType;
//                        var parentRestrictions = mt.IsAbstract || !mt.IsClass;

//                        if (parentRestrictions)
//                            throw new ArgumentOutOfRangeException($"{nameof(AttributeModulesCreate)}: Cannot create module: {mt.Name} is abstract or is not a class.");

//                        serv = ServiceHarnessHelper.DefaultCreator(mt)() as IApiModuleService;

//                        setmi.Invoke(_ctx, new object[] { serv });
//                    }
//                    catch (Exception ex)
//                    {

//                        throw;
//                    }

//                }
//            }
//        }
//        #endregion
//        #region AttributeModulesLoad()
//        /// <summary>
//        /// This method will connect any module set for automatic connection to the main context.
//        /// It will also create a default logger based on the module type.
//        /// </summary>
//        /// <param name="lf">The logging factory.</param>
//        public void AttributeModulesLoad()
//        {
//            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.Load))
//            {
//                var serv = MethodInfoInvokeContext(mi);
//                serv.Load(_ctx);
//            }
//        }
//        #endregion

//        #region AttributeModulesConnect(ILoggerFactory lf)
//        /// <summary>
//        /// This method will connect any module set for automatic connection to the main context.
//        /// It will also create a default logger based on the module type.
//        /// </summary>
//        /// <param name="lf">The logging factory.</param>
//        public void AttributeModulesConnect(ILoggerFactory lf)
//        {
//            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.Connect))
//            {
//                var serv = MethodInfoInvokeContext(mi);
//                serv.Connect(_ctx, lf.CreateLogger(mi.ReturnType));
//            }
//        }
//        #endregion

//        #region AttributeModulesStart(CancellationToken cancellationToken)
//        /// <summary>
//        /// This module will start any module marked for automatic start.
//        /// </summary>
//        public Task AttributeModulesStart(CancellationToken cancellationToken) => 
//            Task.WhenAll(AttributeModuleStartStopExtract(ModuleStartStopMode.Start).Select(m => m.Start(cancellationToken)));
//        #endregion
//        #region AttributeModulesStop(CancellationToken cancellationToken)
//        /// <summary>
//        /// This method will stop any module set to stop automatically.
//        /// </summary>
//        /// <returns></returns>
//        public Task AttributeModulesStop(CancellationToken cancellationToken) => 
//            Task.WhenAll(AttributeModuleStartStopExtract(ModuleStartStopMode.Stop).Select(m => m.Stop(cancellationToken)));
//        #endregion

//        #region AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
//        /// <summary>
//        /// This method examines the context and extracts any start/stop declarations.
//        /// </summary>
//        /// <returns>Returns the list of declarations.</returns>
//        private IEnumerable<MethodInfo> AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
//        {
//            //Filter for the attribute types that we wish to get.
//            var results = AttributeDataGet(attrFilterStartStop);

//            foreach (var result in results)
//            {
//                if (result.attr.Length == 0)
//                    continue;

//                var attrMode = AttributeModuleStartStopAttributeModeExtract(result.attr[0]);
//                if (!attrMode.HasValue || (attrMode.Value & mode) == 0)
//                    continue;

//                yield return result.mi;
//            }

//            yield break;
//        }
//        private static ModuleStartStopMode? AttributeModuleStartStopAttributeModeExtract(CustomAttributeData data)
//        {
//            if (data.AttributeType != typeof(ModuleStartStopAttribute))
//                return null;

//            if (data.ConstructorArguments.Count != 1)
//                return null;

//            var arg = data.ConstructorArguments[0];
//            if (arg.ArgumentType != typeof(ModuleStartStopMode))
//                return null;

//            return (ModuleStartStopMode)arg.Value;
//        }
//        #endregion
//        #region AttributeModuleStartStopExtract(ModuleStartStopMode mode)
//        /// <summary>
//        /// This method examines the context and extracts any start/stop declarations.
//        /// </summary>
//        /// <returns>Returns the list of declarations.</returns>
//        private IEnumerable<IApiModuleService> AttributeModuleStartStopExtract(ModuleStartStopMode mode)
//        {
//            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(mode))
//            {
//                var obj = MethodInfoInvokeContext(mi);

//                if (obj != null)
//                    yield return obj;
//            }

//            yield break;
//        }

//        private IApiModuleService MethodInfoInvokeContext(MethodInfo mi) 
//        {
//            var obj = mi.Invoke(_ctx, new object[] { });

//            if (obj is IApiModuleService)
//                return obj as IApiModuleService;

//            return null;
//        }


//        #endregion
//    }
//}