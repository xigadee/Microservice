#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract class PartitionConfig
    {
        protected internal PartitionConfig(int id)
        {
            Id = id;
        }

        /// <summary>
        /// This is the numeric partition id.
        /// </summary>
        public int Id { get; set; }
    }
}
