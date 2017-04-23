#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public class EntityCacheResult<E>
    {
        public bool Success { get; set; }

        public bool Exists { get; set; }

        public E Entity { get; set; }
    }
}
