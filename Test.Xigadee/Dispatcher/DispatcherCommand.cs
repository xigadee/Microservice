using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{

    /// <summary>
    /// This command is used to verify the Dispatcher functionality.
    /// </summary>
    public class DispatcherCommand:CommandBase
    {
        /// <summary>
        /// This is the standard event handler.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnTestCommandReceive;

        /// <summary>
        /// This command raises an event when it is called.
        /// </summary>
        /// <param name="inPayload">The incoming payload.</param>
        [CommandContract("friday", "standard")]
        private void TestReceive(TransmissionPayload inPayload, List<TransmissionPayload> outCommands)
        {
            OnTestCommandReceive?.Invoke(this, inPayload);
        }

        [CommandContract("friday", "feeling")]
        [return: PayloadOut]
        private string TestCommand([PayloadIn] string inData)
        {
            var principal = Thread.CurrentPrincipal;
            return "Thanks for all the fish";
        }

        /// <summary>
        /// This command raises an unhandled exception when it is called.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        [CommandContract("friday", "exception")]
        [return: PayloadOut]
        private string TestException([PayloadIn] string inData)
        {
            throw new Exception("This is the default exception");
        }
    }
}
