using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class EventTestCommand<I>: CommandBase
        where I : IMessageContract
    {
        /// <summary>
        /// This is the event that the command can be attached to.
        /// </summary>
        public event EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>> OnExecute;

        public EventTestCommand() : base()
        {

        }

        public override void CommandsRegister()
        {
            base.CommandsRegister();
            CommandRegister<I>(ExecuteRequest);
        }

        /// <summary>
        /// This method is called when an entity of the cache type is updated or deleted.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <returns></returns>
        protected virtual async Task ExecuteRequest(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            if (OnExecute != null)
                OnExecute(this, new Tuple<TransmissionPayload, List<TransmissionPayload>>(rq, rs));
        }
    }

}
