using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    public static class TypeHelper
    {

        public static Type Resolve(string typeAsString, bool throwExceptionOnNotResolved = false)
        {
            if (string.IsNullOrEmpty(typeAsString))
                throw new ArgumentNullException($"{nameof(typeAsString)}");

            var typeparts = typeAsString.Split(',');

            return Resolve(typeparts[0], throwExceptionOnNotResolved, typeparts.Length > 2?typeparts[1]:null);
        }

        public static Type Resolve(string className, bool throwExceptionOnNotResolved, string assemblyName = null)
        {
            Assembly[] ass_s;

            if (string.IsNullOrEmpty(assemblyName))
                ass_s = AppDomain.CurrentDomain.GetAssemblies();
            else
                ass_s = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith(assemblyName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

            var types = ass_s.SelectMany(t => t.DefinedTypes)
                .Where(t => t.FullName.StartsWith(className.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .Select(t => t.AsType())
                .ToArray();

            return types[0];
        }

        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type rawGenericType)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (rawGenericType == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
