using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception will be thrown when an invalid name or identifier is used for the Microservice.
    /// </summary>
    public class MicroserviceIdNotValidException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="paramValue">The parameter value.</param>
        public MicroserviceIdNotValidException(string paramName, string paramValue)
            :base($"The parameter '{paramName}' contains invalid characters. Only A-Za-z0-9 are allowed.")
        {
            ParamName = paramName;
            ParamValue = paramValue;
        }
        /// <summary>
        /// This is the parameter that contains illegal characters.
        /// </summary>
        public string ParamName { get; }
        /// <summary>
        /// This is the value passed.
        /// </summary>
        public string ParamValue { get; }

    }
}
