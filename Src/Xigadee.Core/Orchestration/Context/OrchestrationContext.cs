using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The orchestration context is used to hold the process state as it passes through the orchestration flow.
    /// </summary>
    public class OrchestrationContext
    {
        /// <summary>
        /// This is the unique Id for the context.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// This is the version Id for the context.
        /// </summary>
        public Guid VersionId { get; set; } = Guid.NewGuid();
        /// <summary>
        /// This is the flow Id that the context is based on.
        /// </summary>
        public Guid OrchestrationId { get; set; } 

        /// <summary>
        /// This is the date when the context was created.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// This is the last update for the context.
        /// </summary>
        public DateTime? DateUpdated { get; set; }
    }
}
