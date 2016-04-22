using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class SearchRequest: SearchOData4Base, IEquatable<SearchRequest>
    {
        public bool Equals(SearchRequest other)
        {
            return true;
        }
    }
}
