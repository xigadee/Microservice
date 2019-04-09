using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
        readonly Func<CustomAttributeData, bool> attrFilterRepo = (d) =>
            d.AttributeType == typeof(RepositoriesProcessAttribute) ||
            d.AttributeType == typeof(StopRepositoriesProcessAttribute)
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
        {
            var resultM = _ctx.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
                .Select((m) => (m.CustomAttributes.Where(attrFilter).ToArray(), m))
                ;

            var resultP = _ctx.GetType()
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
        public IEnumerable<(Type sType, object service)> RepositoryProcessExtract()
        {
            //Filter for the attribute types that we wish to get.
            var results = GetAttributeData(attrFilterRepo);

            //foreach (var result in results)
            //{
            //    var attrs = result.Item1.ToList();

            //    if (attrs.Contains((a) => a.AttributeType == typeof(DoNotRegisterAsSingletonAttribute)))
            //    {
            //        //TODO: OK, as we may have multiple attributes, with deny set for a specific registration type, 
            //        //we need to filter out the ones registered and then adjust the collection.
            //        continue;
            //    }

            //    //Is the stop attribute defined, if so skip.
            //    if (attrs.Count == 0)
            //        continue;

            //    //Ok, extract the object and return the type. We may return multiple registrations for a single property.
            //    object item = result.Item2.Invoke(_ctx, new object[] { });

            //    if (item != null)
            //        foreach (CustomAttributeData ad in attrs)
            //        {
            //            //OK, we know this is a SingletonRegistrationAttribute, so we just need to get the constructor parameter
            //            if (ad.ConstructorArguments.Count == 0 || ad.ConstructorArguments[0].Value == null)
            //                yield return (result.Item2.ReturnType, item);
            //            else
            //                yield return ((Type)ad.ConstructorArguments[0].Value, item);
            //        }
            //}

            yield break;
        }
        #endregion

    }
}
