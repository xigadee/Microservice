using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by filters that require access to the Microservice to operate.
    /// </summary>
    public interface IRequireMicroserviceConnection
    {
        /// <summary>
        /// This is the link to the Microservice.
        /// </summary>
        IMicroservice Microservice { get; set; }
    }
}
