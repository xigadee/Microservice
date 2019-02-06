using System;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class FabricConnectionBase<M>
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

    }
}
