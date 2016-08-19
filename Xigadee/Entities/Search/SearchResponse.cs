using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class SearchResponse: SearchOData4Base
    {
        public string Etag { get; set; }

        public Dictionary<int, string> Fields { get; } = new Dictionary<int, string>();

        public List<string[]> Data { get; set;} 

    }
}
