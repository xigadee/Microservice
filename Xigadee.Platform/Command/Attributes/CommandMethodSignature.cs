using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to marshall the method signature
    /// </summary>
    [DebuggerDisplay("{Method.Name}")]
    public class CommandMethodSignature:IEnumerable<CommandHolder>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="info">The method for the signature.</param>
        /// <param name="throwSignatureException">This property specifies whether the system should throw exceptions if a signature is invalid.</param>
        public CommandMethodSignature(ICommand command, MethodInfo info, bool throwSignatureException = false)
        {
            if (info == null)
                throw new CommandMethodSignatureException($"Constructor error - info cannot be null.");

            if (command == null)
                throw new CommandMethodSignatureException($"Constructor error - command cannot be null.");

            Command = command;

            Method = info;

            IsValid = Validate(throwSignatureException);
        }

        public ICommand Command { get; }

        public MethodInfo Method { get; }

        public bool IsGeneric { get; set; }

        public bool IsValid { get; }

        public List<CommandContractAttribute> CommandAttributes { get; protected set; }

        public List<ParameterInfo> Parameters { get; protected set;}

        protected virtual bool Validate(bool throwException = false)
        {

            CommandAttributes = Attribute.GetCustomAttributes(Method)
                .Where((a) => a is CommandContractAttribute)
                .Cast<CommandContractAttribute>()
                .ToList();

            //This shouldn't happen, but check anyway.
            if (CommandAttributes.Count == 0)
                throw new CommandMethodSignatureException($"CommandAttributes are not defined for the method.");

            Parameters = Method.GetParameters().ToList();

            bool isGeneric = true;

            //var genericIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            //if (genericIn != null)
            //    isGeneric &= paramInfo.Remove(genericIn);

            //var genericOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));
            //if (genericOut != null)
            //    isGeneric &= paramInfo.Remove(genericOut) && paramInfo.Count == 0;

            ////Ok, this is a generci method so we can quit now.
            //if (isGeneric && RegisterGenericSignature(commandAttrs, info, genericIn, genericOut))
            //    return;


            ////Ok, let's get the request parameter
            //var rqAttr = paramInfo.Select((p) => ParamAttributes<PayloadInAttribute>(p)).FirstOrDefault();
            //if (rqAttr != null)
            //    paramInfo.Remove(rqAttr.Item1);

            ////And finally the response parameter
            //var rsAttr = paramInfo.Select((p) => ParamAttributes<PayloadOutAttribute>(p)).FirstOrDefault();
            //if (rsAttr != null)
            //    paramInfo.Remove(rsAttr.Item1);

            return true;
        }

        public string Reference(CommandContractAttribute attr)
        {
            return $"{Method.Name}/{attr.Header.ToKey()}";
        }

        public Func<TransmissionPayload, List<TransmissionPayload>, Task> Action
        {
            get; protected set;
        }

        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandard(MethodInfo info)
        {
            return async (pIn, pOut) =>
            {
                await (Task)info.Invoke(Command, new object[] { pIn, pOut });
            };
        }

        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandardParameter(MethodInfo info, Type paramInType)
        {
            return async (pIn, pOut) =>
            {
                await (Task)info.Invoke(Command, new object[] { pIn, pOut });
            };
        }

        public IEnumerator<CommandHolder> GetEnumerator()
        {
            return null;
            ////Register a command for each of the attributes defined.
            //return CommandAttributes.Select((a) => new CommandHolder(
            //    CommandRegister(CommandChannelAdjust(a), signature.Action, referenceId: signature.Reference(a))
            //    );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// This method replaces the channel with the command default if the value specified in the attribute is null.
        /// </summary>
        /// <param name="attr">The incoming attribute whose header channel should be checked.</param>
        /// <returns>Returns a message filter wrapper for the header.</returns>
        protected MessageFilterWrapper CommandChannelAdjust(CommandContractAttribute attr)
        {
            ServiceMessageHeader header = attr.Header;
            if (header.ChannelId == null)
                header = new ServiceMessageHeader(Command.ChannelId, header.MessageType, header.ActionType);

            return new MessageFilterWrapper(header);
        }

    }
}
