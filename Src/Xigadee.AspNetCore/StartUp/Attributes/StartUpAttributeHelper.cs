using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This helper class extracts the attributes from the class for registration.
    /// </summary>
    public static class StartUpAttributeHelper
    {
        /// <summary>
        /// This method extracts the singleton registrations from the context.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IEnumerable<(Type sType, object service)> SingletonRegistrationsExtract(this ApiStartUpContext ctx)
        {
            //Filter for the attribute types that we wish to get.
            Func<CustomAttributeData, bool> attrFilter = (d) =>
                d.AttributeType == typeof(SingletonRegistrationAttribute) ||
                d.AttributeType == typeof(StopSingletonRegistrationAttribute)
                ;

            var resultM = ctx.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
                .Select((m) => (m.CustomAttributes, m))
                ;

            var resultP = ctx.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Contains((a) => attrFilter(a)))
                .Select((m) => (m.CustomAttributes, m.GetGetMethod()))
                ;

            foreach (var result in resultM.Union(resultP))
            {
                //Is the stop attribute defined, if so skip.
                if (result.Item1.Contains((a) => a.AttributeType == typeof(StopSingletonRegistrationAttribute)))
                    continue;

                //Ok, extract the object and return the type. We may return multiple registrations for a single property.
                object item = result.Item2.Invoke(ctx, new object[] { });

                if (item != null)
                    foreach (CustomAttributeData ad in result.Item1)
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
    }

    /// <summary>
    /// This exception is throw if there is an error during singleton extraction
    /// </summary>
    public class SingletonRegistrationAttributeException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SingletonRegistrationAttributeException(string message):base(message)
        {

        }
    }
}
