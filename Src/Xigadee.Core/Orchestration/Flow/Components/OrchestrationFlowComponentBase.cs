using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class contains the base properties for the orchestration components.
    /// </summary>
    public abstract class OrchestrationFlowComponentBase
    {
        /// <summary>
        /// This is teh uniqueidentifier for the component
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// This property specifies whether the component is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// This is the timestamp that the component was created
        /// </summary>
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// This is the timestamp that the component was updated.
        /// </summary>
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// This is the friendly name for the component.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This is the optional friendly description of the component.
        /// </summary>
        public string Description { get; set; }
    }
}
