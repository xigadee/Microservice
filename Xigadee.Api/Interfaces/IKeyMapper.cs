using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.Api
{
    public interface IKeyMapper<K>
    {
        K ToKey(string value);

        string ToString(K key);
    }
}
