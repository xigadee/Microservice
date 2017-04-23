using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    [DebuggerDisplay("ResourceProfile: {Id}")]
    public class ResourceProfile
    {
        public ResourceProfile(string id)
        {
            Id = id;
        }
        /// <summary>
        /// This is the Id of the resource profile.
        /// </summary>
        public string Id { get; set; }
    }
}
