using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class ApiWrapper<K,E> : WrapperBase<K,E>
        where K: IEquatable<K>
    {
        public override ServiceStatus Status { get { return ServiceStatus.Created; } }

        public override string Name { get; protected set; }

        public override void Start()
        {
            //throw new NotImplementedException();
        }

        public override void Stop()
        {
            //throw new NotImplementedException();
        }
    }
}
