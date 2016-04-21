using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class MemoryPersistenceDirectiveRequest: IEquatable<MemoryPersistenceDirectiveRequest>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool Equals(MemoryPersistenceDirectiveRequest other)
        {
            return other.Id == Id;
        }
    }

    public class MemoryPersistenceDirectiveResponse
    {
    }
}
