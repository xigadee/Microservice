using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the core entity that define the process through the orchestration flow.
    /// </summary>
    public class OrchestrationFlow: OrchestrationFlowComponentBase
    {
        /// <summary>
        /// This is the defaul con
        /// </summary>
        public OrchestrationFlow()
        {

        }

        /// <summary>
        /// This is the entity version Id.
        /// </summary>
        public Guid VersionId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// This sets the process priority. False = inherits from calling process, true then the context priority will be set to 
        /// the value: 0 => batch; 1 = real time; 2 = highest
        /// </summary>
        public ValueSwitch<int> ContextPriorityOverride { get; set; } = ValueSwitch<int>.Default;

        /// <summary>
        /// This is the class that contains the validation pass criteria.
        /// </summary>
        public OrchestrationFlowValidator Validator { get; set; } = new OrchestrationFlowValidator();
        /// <summary>
        /// This is the flow path when the validation has passed.
        /// </summary>
        public OrchestrationFlowPath PathValidatePass { get; set; } = new OrchestrationFlowPath();
        /// <summary>
        /// This is the flow path when the validation has failed.
        /// </summary>
        public OrchestrationFlowPath PathValidateFail { get; set; } = new OrchestrationFlowPath();



    }
}
