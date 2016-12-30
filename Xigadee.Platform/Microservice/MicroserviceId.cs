using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        /// This method validates that a name is legal. This is that the name only contains A-Za-z0-9
        /// </summary>
        /// <param name="id">The string to validate.</param>
        /// <returns>Returns true if the name is legal.</returns>
        public static bool ValidServiceIdentifier(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            // "^[A-Za-z0-9_]+$" is the Regex but this works just fine.

            return id.Length == id.ToLowerInvariant().ToCharArray()
                .Where((c) => c>='a' & c<='z' | c>='0' & c<= '9')
                .Count();
        }
        /// <summary>
        /// This is the default constructor for the Microservice Id object.
        /// </summary>
        /// <param name="name">The Microservice short name.</param>
        /// <param name="serviceId">The service unique GUID</param>
        /// <param name="description">An optional description for the Microservice.</param>
        /// <param name="externalServiceId">The external service id. </param>
        /// <param name="serviceVersionId">The service version id. If not set, this will be pulled from the calling assembly Dll settings.</param>
        /// <param name="serviceEngineVersionId">The engine version id. If not set, this will be pulled from the executing assembly Dll settings.</param>
        /// <param name="properties">Any custom additional properties.</param>
        public MicroserviceId(string name
            , string serviceId = null
            , string description = null
            , string externalServiceId = null
            , string serviceVersionId = null
            , string serviceEngineVersionId = null
            , IEnumerable<Tuple<string,string>> properties = null)
        {
            ServiceId = string.IsNullOrEmpty(serviceId) ? Guid.NewGuid().ToString("N").ToUpperInvariant() : serviceId;
            Name = string.IsNullOrEmpty(name) ? $"Service{ServiceId}" : name;

            if (!ValidServiceIdentifier(ServiceId))
                throw new MicroserviceIdNotValidException(nameof(ServiceId), ServiceId);

            if (!ValidServiceIdentifier(Name))
                throw new MicroserviceIdNotValidException(nameof(Name), Name);

            Description = description;
            StartTime = DateTime.UtcNow;
            MachineName = Environment.MachineName;
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
        /// This is an optional description for the Microservice.
        /// </summary>
        public string Description { get; }
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
