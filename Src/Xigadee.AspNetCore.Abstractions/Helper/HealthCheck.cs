using System;

namespace Xigadee
{
    /// <summary>
    /// This class handles health check. Basically you pass a predefined Guid to the function, and it will return a response indicating that 
    /// the system is active. This is used by applications such as Application Insight to poll the service.
    /// </summary>
    public class ConfigHealthCheck
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConfigHealthCheck"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the ID, used to identify a valid request.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Specifies that we should show the Microservice statistics are part of the healthcheck.
        /// </summary>
        public bool ShowStatistics { get; set; }
        /// <summary>
        /// This is the path to register the healthcheck under. If this is null or empty, then this will be registered under healthcheck.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// This class is used to match an incoming id for a health check request.
    /// </summary>
    public static class HealthCheckSettingsHelper
    {
        /// <summary>
        /// Validates the incoming Id.
        /// </summary>
        /// <param name="settings">The health check settings.</param>
        /// <param name="id">The incoming id.</param>
        /// <returns>Returns true if matched.</returns>
        public static bool Validate(this ConfigHealthCheck settings, string id)
        {
            if (settings == null)
                return false;

            if (!settings.Enabled)
                return false;

            if (!settings.Id.HasValue && string.IsNullOrEmpty(id))
                return true;

            Guid value;
            if (!Guid.TryParse(id, out value))
                return false;

            return settings.Id.Value == value;
        }
    }
}
