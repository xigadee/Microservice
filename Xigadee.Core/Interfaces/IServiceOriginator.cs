using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IServiceOriginator
    {
        /// <summary>
        /// The originator Id for the service.
        /// </summary>
        string OriginatorId { get; set; }
    }
}
