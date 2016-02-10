#region using
using IMGroup.Core;
using IMGroup.Core.Persistence;
using IMGroup.Core.Persistence.DocumentDb;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IMGroup.Microservice.Persistence;
#endregion
namespace IMGroup.Microservice
{
    /// <summary>
    /// This is the file response holder.
    /// </summary>
    public class FileResponseHolder:IPersistenceResponseHolder
    {

        public byte[] Content { get; set; }

        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

        public bool IsTimeout { get; set; }

        public string VersionId { get; set; }

        public Exception Ex { get; set; }
    }
}
