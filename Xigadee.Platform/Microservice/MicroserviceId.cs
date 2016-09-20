using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds the Microservice identifier information in a central place.
    /// </summary>
    [DebuggerDisplay("{ExternalServiceId}")]
    public class MicroserviceId
    {
        /// <summary>
        /// This is the default constructor for the Microservice Id object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serviceId"></param>
        /// <param name="externalServiceId"></param>
        /// <param name="serviceVersionId"></param>
        /// <param name="serviceEngineVersionId"></param>
        /// <param name="properties">The initial properties.</param>
        public MicroserviceId(string name
            , string serviceId = null
            , string externalServiceId = null
            , string serviceVersionId = null
            , string serviceEngineVersionId = null
            , IEnumerable<Tuple<string,string>> properties = null)
        {
            StartTime = DateTime.UtcNow;

            Name = name;
            MachineName = Environment.MachineName;
            ServiceId = string.IsNullOrEmpty(serviceId) ? Guid.NewGuid().ToString("N").ToUpperInvariant() : serviceId;

            ServiceVersionId = serviceVersionId ?? Assembly.GetCallingAssembly().GetName().Version.ToString();
            ServiceEngineVersionId = serviceEngineVersionId ?? Assembly.GetExecutingAssembly().GetName().Version.ToString();

            ExternalServiceId = externalServiceId ?? string.Format("{0}_{1}_{2:yyyyMMddHHmm}_{3}", Name, MachineName, StartTime, ServiceId);

            Properties = properties?.ToDictionary((i) => i.Item1, (i) => i.Item2)??new Dictionary<string, string>();
        }
        /// <summary>
        /// This is the UTC start time of the service.
        /// </summary>
        public DateTime StartTime { get; }
        /// <summary>
        /// This is the friendly name of the service.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// This is the underlying machine name that hosts the service.
        /// </summary>
        public string MachineName { get; }
        /// <summary>
        /// This is the unique Id of the Microservice.
        /// </summary>
        public string ServiceId { get; }
        /// <summary>
        /// This is the Dll version Id of the calling assembly
        /// </summary>
        public string ServiceVersionId { get; }
        /// <summary>
        /// This is the Dll version is of Xigadee assembly
        /// </summary>
        public string ServiceEngineVersionId { get; }
        /// <summary>
        /// This is the unique reference assembly. This id is used for message routing.
        /// </summary>
        public string ExternalServiceId { get; }
        /// <summary>
        /// This dictionary holds a set of additional properties that identify the Microservice.
        /// </summary>
        public Dictionary<string, string> Properties { get; }
    }
}
