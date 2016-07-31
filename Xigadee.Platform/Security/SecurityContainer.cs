using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class SecurityContainer: ServiceContainerBase<SecurityStatistics, SecurityPolicy>
    {
        public SecurityContainer(SecurityPolicy policy):base(policy)
        {

        }

        protected override void StartInternal()
        {
            //throw new NotImplementedException();
        }

        protected override void StopInternal()
        {
            //throw new NotImplementedException();
        }
    }
}
