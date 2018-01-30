using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the authentication collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerAuthentication> Authentication { get; }

        private void OnAuthenticationAdd(IServiceHandlerAuthentication handler)
        {
            handler.Collector = Collector;
            handler.OriginatorId = OriginatorId;
        }



    }
}
