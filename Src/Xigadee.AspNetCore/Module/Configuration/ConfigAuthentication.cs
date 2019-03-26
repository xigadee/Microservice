namespace Xigadee
{
    /// <summary>
    /// This is the default configuration settings for the application authentication.
    /// </summary>
    public abstract class ConfigAuthentication
    {
        /// <summary>
        /// Gets or sets the name for the authentication method.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether connections can only be https.
        /// </summary>
        public bool HttpsOnly { get; set; }

        /// <summary>
        /// Gets the system environment name, i.e. dev, uat, prod etc.
        /// </summary>
        public string EnvironmentName { get; private set; }
        /// <summary>
        /// Sets the name of the environment.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SetEnvironmentName(string name) => EnvironmentName = name;
    }
}
