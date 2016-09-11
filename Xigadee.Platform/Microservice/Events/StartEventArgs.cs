using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to signal the start events.
    /// </summary>
    public class StartEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// The current configuration options.
        /// </summary>
        public MicroserviceConfigurationOptions ConfigurationOptions { get; set; }
    }

}
