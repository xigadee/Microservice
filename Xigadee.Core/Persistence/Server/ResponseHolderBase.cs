using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMGroup.Microservice
{
    public class ResponseHolderBase: IPersistenceResponseHolder
    {
        public byte[] Content { get; set; }


        public Exception Ex { get; set; }


        public bool IsSuccess { get; set; }


        public bool IsThrottled { get; set; }


        public bool IsTimeout { get; set; }


        public int StatusCode { get; set; }


        public string VersionId { get; set; }

    }
}
