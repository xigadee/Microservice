using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The security container class contains all the components to secure the incoming messaging for a Microservice, 
    /// and to ensure that incoming message requests have the correct permissions necessary to be processed.
    /// </summary>
    public class SecurityContainer: ServiceContainerBase<SecurityStatistics, SecurityPolicy>
    {
        public SecurityContainer(SecurityPolicy policy):base(policy)
        {

        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
