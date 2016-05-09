using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract class used by the primary Microservice containers.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class ServiceContainerBase<S,P>:ServiceBase<S>
        where S : StatusBase, new()
        where P : PolicyBase, new()
    {
        /// <summary>
        /// This is the container policy.
        /// </summary>
        protected P mPolicy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="name"></param>
        public ServiceContainerBase(P policy = null, string name = null):base(name)
        {
            mPolicy = policy ?? new P();
        }
    }
}
