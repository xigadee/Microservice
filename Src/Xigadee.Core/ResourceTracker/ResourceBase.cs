using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class ResourceBase: IResourceBase
    {
        private Guid mId = Guid.NewGuid();
        protected string mName;

        public Guid ResourceId
        {
            get
            {
                return mId;
            }
        }

        public string Name
        {
            get{ return mName; }
        }
    }
}
