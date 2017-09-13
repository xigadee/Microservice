using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class contains helpers and extension methods for the ServiceHarness.
    /// </summary>
    public static class ServiceHarnessHelper
    {
        #region DefaultCreator<T>()
        /// <summary>
        /// This method checks whether the command supports a parameterless constructor, or a constructor with all optional parameters.
        /// </summary>
        /// <returns>Returns the command.</returns>
        public static Func<T> DefaultCreator<T>()
        {
            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);

            if (constructor != null)
                return () => Activator.CreateInstance<T>();
            
            //Get the first constructor that supports all optional parameters (i.e. parameterless) with the least amount of parameters.
            constructor = typeof(T)
                .GetConstructors()
                .Where((c) => c.GetParameters().All((p) => p.IsOptional))
                .OrderBy((c) => c.GetParameters().Count())
                .FirstOrDefault();
            
            if (constructor == null)
                throw new ArgumentOutOfRangeException($"The object {typeof(T).Name} does not support a parameterless constructor, or a constructor with optional parameters. Please supply a specific creator function.");
            
            //Create an array of all Type.Missing for each of the optional parameters.
            var parameters = Enumerable.Range(0, constructor.GetParameters().Count()).Select((i) => Type.Missing).ToArray();

            return () => (T)Activator.CreateInstance(typeof(T)
                    , BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding
                    , null
                    , parameters
                    , CultureInfo.CurrentCulture
                    );
        }
        #endregion
    }
}
