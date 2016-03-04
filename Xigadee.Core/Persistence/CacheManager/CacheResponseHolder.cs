using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceResponseHolder<E>: PersistenceResponseHolder, IResponseHolder<E>
    {
        public E Entity
        {
            get; set;
        }
    }
    public class PersistenceResponseHolder: IResponseHolder
    {
        public PersistenceResponseHolder()
        {
            Fields = new Dictionary<string, string>();
        }

        public string Content
        {
            get;set;
        }


        public Exception Ex
        {
            get;set;
        }

        public Dictionary<string, string> Fields
        {
            get;set;
        }

        public string Id
        {
            get;set;
        }

        public bool IsSuccess
        {
            get;set;
        }

        public bool IsTimeout
        {
            get;set;
        }

        public int StatusCode
        {
            get;set;
        }

        public string VersionId
        {
            get;set;
        }
    }
}
