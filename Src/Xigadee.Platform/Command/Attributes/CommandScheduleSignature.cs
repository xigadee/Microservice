using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to marshal the method signature for a in-line schedule command.
    /// </summary>
    [DebuggerDisplay("{Method.Name}")]
    public class CommandScheduleSignature: CommandSignatureBase
    {
        /// <summary>
        /// This specifies whether the method call is async.
        /// </summary>
        public bool IsAsync { get; private set; }

        /// <summary>
        /// This is the list of the parameters for the method.
        /// </summary>
        public List<ParameterInfo> Parameters { get; protected set; }

        /// <summary>
        /// This method validates the method.
        /// </summary>
        /// <param name="throwException">Set this to true throw an exception if the signature does not match,</param>
        /// <returns>Returns true if the signature is validated.</returns>
        protected override bool Validate(bool throwException = false)
        {
            try
            {
                //OK, check whether the return parameter is a Task or Task<> async construct
                IsAsync = typeof(Task).IsAssignableFrom(Method.ReturnParameter.ParameterType);

                Parameters = Method.GetParameters().ToList();
                var paramInfo = Method.GetParameters().ToList();

                //OK, see if the standard parameters exist and aren't decorated as In or Out.
                InSchedule = GetParamPos(Parameters, paramInfo, typeof(Schedule));
                InCancellationToken = GetParamPos(Parameters, paramInfo, typeof(CancellationToken));

                //Now check the out parameter is acceptable
                if (IsAsync)
                {
                    if (Method.ReturnParameter.ParameterType != typeof(Task))
                        throw new CommandContractSignatureException($"Generic Task response parameter can only have one parameter.");
                }
                else
                {
                    if (Method.ReturnParameter.ParameterType != typeof(void))
                        throw new CommandContractSignatureException($"Generic Task response parameter can only have one parameter.");
                }

                //Finally check that we have used all the parameters.
                if (paramInfo.Count != 0 && throwException)
                    throw new CommandContractSignatureException($"There are too many parameters in the method ({paramInfo[0].Name}).");

                return paramInfo.Count == 0;

            }
            catch (Exception ex)
            {
                throw new CommandContractSignatureException("Incorrect method declaration.", ex);
            }
        }

        /// <summary>
        /// This is the StandardIn parameter
        /// </summary>
        public (bool success, ParameterInfo param, int? pos) InSchedule { get; private set; }
        /// <summary>
        /// This is the StandardIn parameter
        /// </summary>
        public (bool success, ParameterInfo param, int? pos) InCancellationToken { get; private set; }

        /// <summary>
        /// This is the command action that is executed.
        /// </summary>
        public Func<Schedule, CancellationToken, Task> Action
        {
            get
            {
                return async (schedule, token) =>
                {
                    var collection = new object[Parameters.Count];

                    if (InSchedule.success)
                        collection[InSchedule.pos.Value] = schedule;

                    if (InCancellationToken.success)
                        collection[InCancellationToken.pos.Value] = token;

                    if (IsAsync)
                    {
                        await (Task)Method.Invoke(Command, collection);
                    }
                    else
                    {
                        Method.Invoke(Command, collection);
                    }
                };
            }
        }
    }
}