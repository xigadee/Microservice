using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This enumeration records the creation cycle for the application.
    /// </summary>
    public enum ApiModuleLifecycle : int
    {
        Initialized = 0,
        Beginning = 1,
        Created = 2,
        Loaded = 3,
        MicroserviceConfigured = 4,
        Connected = 5,
        Starting = 6,
        Started = 7
    }
}
