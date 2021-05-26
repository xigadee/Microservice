using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    //Attributes
    public abstract partial class ApiStartUpContextRoot<HE>
    {
        #region Declarations
        /// <summary>
        /// Filter for the Repositories process attribute types that we wish to get.
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterRepoRoot = (d) =>
            d.AttributeType == typeof(RepositoriesProcessAttribute) ||
            d.AttributeType == typeof(StopRepositoriesProcessAttribute)
            ;

        /// <summary>
        /// This definition is used to pull out the configuration module settings.
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterConfiguration = (d) =>
            d.AttributeType == typeof(ConfigurationSetAttribute) ||
            d.AttributeType == typeof(DoNotConfigurationSetAttribute)
            ;

        /// <summary>
        /// Filter for the repository load attribute.
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterRepoClass = (d) =>
            d.AttributeType == typeof(RepositoryLoadAttribute)
            //|| d.AttributeType == typeof(StopRepositoriesProcessAttribute)
            ;

        /// <summary>
        /// Filter for the attribute types that we wish to get. 
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterStartStop = (d) =>
            d.AttributeType == typeof(ModuleStartStopAttribute)
            ;

        /// <summary>
        /// Filter for the attribute types that we wish to get.
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterSingleton = (d) =>
            d.AttributeType == typeof(RegisterAsSingletonAttribute) ||
            d.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)
            ;

        /// <summary>
        /// Filter for the attribute types that we wish to get.
        /// </summary>
        protected readonly Func<CustomAttributeData, bool> attrFilterConfigurationSet = (d) =>
            d.AttributeType == typeof(ConfigurationSetAttribute) ||
            d.AttributeType == typeof(DoNotConfigurationSetAttribute)
            ;
        #endregion

        #region AttributeDataGet(Func<CustomAttributeData, bool> attrFilter)
        /// <summary>
        /// This method returns the specific attribute data for property and method declarations.
        /// </summary>
        /// <param name="attrFilter">The specific filter.</param>
        /// <returns>Returns the list of data.</returns>
        protected virtual IEnumerable<(CustomAttributeData[] attr, MethodInfo mi)> AttributeDataGet(Func<CustomAttributeData, bool> attrFilter)
            => AttributeDataGet(GetType(), attrFilter);
        #endregion
        #region AttributeDataGet(Type oType, Func<CustomAttributeData, bool> attrFilter)
        /// <summary>
        /// This method returns the specific attribute data for property and method declarations.
        /// </summary>
        /// <param name="oType">The object type.</param>
        /// <param name="attrFilter">The specific filter.</param>
        /// <returns>Returns the list of data.</returns>
        public static IEnumerable<(CustomAttributeData[], MethodInfo)> AttributeDataGet(Type oType, Func<CustomAttributeData, bool> attrFilter)
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

        #region AttributeConfigurationSetExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public virtual IEnumerable<(Type sType, object service)> AttributeConfigurationSetExtract()
        {
            //Filter for the attribute types that we wish to get.
            var results = AttributeDataGet(attrFilterSingleton);

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
                object item = result.Item2.Invoke(this, new object[] { });

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
        #region AttributeSingletonRegistrationsExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public virtual IEnumerable<(Type sType, object service)> AttributeSingletonRegistrationsExtract()
        {
            //Filter for the attribute types that we wish to get.
            var results = AttributeDataGet(attrFilterSingleton);

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
                object item = result.Item2.Invoke(this, new object[] { });

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
        #region AttributeRepositoryProcessExtract()
        /// <summary>
        /// This method examines the context and extracts any singleton declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        public virtual RepositoryDirectiveCollection AttributeRepositoryProcessExtract()
        {
            var coll = new RepositoryDirectiveCollection();

            //Filter for the attribute types that we wish to get.
            var results = AttributeDataGet(attrFilterRepoRoot);

            foreach (var result in results)
            {
                var attrs = result.Item1.ToList();

                //OK, if there is a stop defined then ignore this module
                if (attrs.Contains((a) => a.AttributeType == typeof(StopRepositoriesProcessAttribute)))
                    continue;

                //Ok, extract the object and we will scan that for the deeper attributes.
                //We need to extract as we have to scan the actual object.
                object item = result.Item2.Invoke(this, new object[] { });

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

        #region AttributeModulesCreate()
        /// <summary>
        /// This metod will create and set any module set for automatic creation.
        /// </summary>
        protected virtual void AttributeModulesCreate()
        {
            var resultP = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilterStartStop(a)))
                //.ToArray()
                ;

            foreach (var pi in resultP)
                AttributeModuleCreate(pi);
        }
        /// <summary>
        /// This method create a specific instance of the module using its parameterless constructor or a constructor only containing optional parameters.
        /// </summary>
        /// <param name="pi">The property info that points to the module property.</param>
        protected virtual bool AttributeModuleCreate(PropertyInfo pi)
        {
            var attr = pi.CustomAttributes.Where(attrFilterStartStop).ToArray();

            if (attr.Length == 0)
                return false;

            var attrMode = AttributeModuleStartStopAttributeModeExtract(attr[0]);
            if (!attrMode.HasValue || (attrMode.Value & ModuleStartStopMode.Create) == 0)
                return false;

            var mi = pi.GetMethod;
            if (mi == null)
                return false;

            var serv = MethodInfoInvokeContext(mi);
            if (serv != null)
                return false;

            var sm = pi.SetMethod;
            if (sm == null)
                return false;

            serv = ServiceHarnessHelper.DefaultCreator(mi.ReturnType)() as IApiModuleService;

            sm.Invoke(this, new object[] { serv });

            return true;
        }
        #endregion
        #region AttributeModulesLoad(IServiceCollection services)
        /// <summary>
        /// This method will load any modules set for automatic connection to the main context.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected virtual void AttributeModulesLoad(IServiceCollection services)
        {
            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.Load))
                AttributeModuleLoad(services, mi);
        }
        /// <summary>
        /// This method will load any module set for automatic connection to the main context.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="mi">The context method info.</param>
        protected virtual void AttributeModuleLoad(IServiceCollection services, MethodInfo mi)
        {
            var serv = MethodInfoInvokeContext(mi);
            serv.Load(this, services);
        }
        #endregion
        #region AttributeModulesMicroserviceConfigure(ILoggerFactory lf)
        /// <summary>
        /// This method will connect any module set for automatic connection to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="lf">The logging factory.</param>
        protected virtual void AttributeModulesMicroserviceConfigure()
        {
            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.MicroserviceConfigure))
                AttributeModuleMicroserviceConfigure(mi);
        }
        /// <summary>
        /// This method will connect a specific module to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="mi">The context method info.</param>
        protected virtual void AttributeModuleMicroserviceConfigure(MethodInfo mi)
        {
            var serv = MethodInfoInvokeContext(mi);
            serv.MicroserviceConfigure();
        }
        #endregion
        #region AttributeModulesConnect(ILoggerFactory lf)
        /// <summary>
        /// This method will connect any module set for automatic connection to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="lf">The logging factory.</param>
        protected virtual void AttributeModulesConnect(ILoggerFactory lf)
        {
            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode.Connect))
                AttributeModuleConnect(mi, lf);
        }
        /// <summary>
        /// This method will connect a specific module to the main context.
        /// It will also create a default logger based on the module type.
        /// </summary>
        /// <param name="mi">The context method info.</param>
        /// <param name="lf">The logging factory.</param>
        protected virtual void AttributeModuleConnect(MethodInfo mi, ILoggerFactory lf)
        {
            var serv = MethodInfoInvokeContext(mi);
            serv.Connect(lf.CreateLogger(mi.ReturnType));
        }
        #endregion

        #region AttributeModulesStart(CancellationToken cancellationToken)
        /// <summary>
        /// This module will start any module marked for automatic start.
        /// </summary>
        protected virtual Task AttributeModulesStart(CancellationToken cancellationToken) =>
            Task.WhenAll(AttributeModuleStartStopExtract(ModuleStartStopMode.Start).Select(m => m.Start(cancellationToken)));
        #endregion
        #region AttributeModulesStop(CancellationToken cancellationToken)
        /// <summary>
        /// This method will stop any module set to stop automatically.
        /// </summary>
        /// <returns></returns>
        protected virtual Task AttributeModulesStop(CancellationToken cancellationToken) =>
            Task.WhenAll(AttributeModuleStartStopExtract(ModuleStartStopMode.Stop).Select(m => m.Stop(cancellationToken)));
        #endregion

        #region AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
        /// <summary>
        /// This method examines the context and extracts any start/stop declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        protected virtual IEnumerable<MethodInfo> AttributeModuleStartStopExtractMethodInfo(ModuleStartStopMode mode)
        {
            //Filter for the attribute types that we wish to get.
            var results = AttributeDataGet(attrFilterStartStop);

            foreach (var result in results)
            {
                if (result.attr.Length == 0)
                    continue;

                var attrMode = AttributeModuleStartStopAttributeModeExtract(result.attr[0]);
                if (!attrMode.HasValue || (attrMode.Value & mode) == 0)
                    continue;

                yield return result.mi;
            }

            yield break;
        }

        /// <summary>
        /// This method extracts the start stop mode from the attribute.
        /// </summary>
        /// <param name="data">The incoming data.</param>
        /// <returns>Returns the extracted mode or null if not set.</returns>
        protected virtual ModuleStartStopMode? AttributeModuleStartStopAttributeModeExtract(CustomAttributeData data)
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
        #region AttributeModuleStartStopExtract(ModuleStartStopMode mode)
        /// <summary>
        /// This method examines the context and extracts any start/stop declarations.
        /// </summary>
        /// <returns>Returns the list of declarations.</returns>
        protected virtual IEnumerable<IApiModuleService> AttributeModuleStartStopExtract(ModuleStartStopMode mode)
        {
            foreach (var mi in AttributeModuleStartStopExtractMethodInfo(mode))
            {
                var obj = MethodInfoInvokeContext(mi);

                if (obj != null)
                    yield return obj;
            }

            yield break;
        }

        /// <summary>
        /// This method invokes the method for this context.
        /// </summary>
        /// <param name="mi">The method infor to invoke.</param>
        /// <returns>Returns the specific module service.</returns>
        protected virtual IApiModuleService MethodInfoInvokeContext(MethodInfo mi)
        {
            var obj = mi.Invoke(this, new object[] { });

            if (obj is IApiModuleService)
                return obj as IApiModuleService;

            return null;
        }


        #endregion

        //Configuration
        #region BindAttributesConfigurationSet()
        /// <summary>
        /// This method reads the ConfigurationSet Attributes and processes them.
        /// </summary>
        protected virtual void BindAttributesConfigurationSet()
        {
            var resultP = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilterConfiguration(a)))
                .Select((m) => (m.CustomAttributes.Where(attrFilterConfiguration).ToArray(), m))
                ;

            foreach (var data in resultP)
                AttributeConfigurationSet(data.Item1, data.m);
        }

        /// <summary>
        /// This method is used to set the configuration for the specific attribute.
        /// </summary>
        /// <param name="aCol">The attribute data.</param>
        /// <param name="pi">The property info.</param>
        protected virtual void AttributeConfigurationSet(CustomAttributeData[] aCol, PropertyInfo pi)
        {
            if (aCol.Length == 0)
                return;

            if (Array.Exists(aCol, a => a.AttributeType == typeof(DoNotConfigurationSetAttribute)))
                return;

            var mi = pi.GetMethod;
            var sm = pi.SetMethod;

            //Read the existing value
            var obj = mi.Invoke(this, new object[] { });
            //If it is already set
            if (obj == null)
            {
                //Create the default object.
                obj = ServiceHarnessHelper.DefaultCreator(mi.ReturnType)();

                //Set the default object in the class.
                pi.SetMethod.Invoke(this, new object[] { obj });
            }

            //OK, create the config using an empty constructor.
            var attr = aCol[0];

            var configName = pi.Name;
            //Bind to the incoming configute.
            if (attr.ConstructorArguments.Count > 0)
                configName = attr.ConstructorArguments[0].Value as string;

            AttributeConfigurationSetBind(configName, obj);
        }
        /// <summary>
        /// This method is used to specifically bind the config to the object
        /// </summary>
        /// <param name="configName">The configuration section name.</param>
        /// <param name="obj">The object to bind the configuration to.</param>
        public abstract void AttributeConfigurationSetBind(string configName, object obj); 
        #endregion
    }
}
