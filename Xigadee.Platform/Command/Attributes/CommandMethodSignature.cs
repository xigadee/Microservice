using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to marshall the method signature
    /// </summary>
    [DebuggerDisplay("{Method.Name}")]
    public class CommandMethodSignature
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="method">The method for the signature.</param>
        /// <param name="throwSignatureException">This property specifies whether the system should throw exceptions if a signature is invalid.</param>
        public CommandMethodSignature(ICommand command, MethodInfo method, bool throwSignatureException = false)
        {
            if (method == null)
                throw new CommandMethodSignatureException($"Constructor error - info cannot be null.");

            if (command == null)
                throw new CommandMethodSignatureException($"Constructor error - command cannot be null.");

            Command = command;

            Method = method;

            IsValid = Validate(throwSignatureException);
        }
        /// <summary>
        /// This is the command connected to the signature
        /// </summary>
        public ICommand Command { get; }
        /// <summary>
        /// This is the specific method.
        /// </summary>
        public MethodInfo Method { get; }
        /// <summary>
        /// Specifies whether this is a generic signature.
        /// </summary>
        public bool IsStandard { get; private set; }

        public bool IsAsync { get; private set; }

        public bool IsReturnValue{ get; private set; }
        /// <summary>
        /// This property specifies whether the signature is valid.
        /// </summary>
        public bool IsValid { get; }
        /// <summary>
        /// These are the assigned command attributes.
        /// </summary>
        public List<CommandContractAttribute> CommandAttributes { get; protected set; }
        /// <summary>
        /// This is the list of the parameters for the method.
        /// </summary>
        public List<ParameterInfo> Parameters { get; protected set;}
        /// <summary>
        /// This method validates the method.
        /// </summary>
        /// <param name="throwException">Set this to true throw an exception if the signature does not match,</param>
        /// <returns>Returns true if the signature is validated.</returns>
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
            var paramInfo = Method.GetParameters().ToList();

            
            //OK, check whether the return parameter is a Task or Task<> construct
            IsAsync = typeof(Task).IsAssignableFrom(Method.ReturnParameter.ParameterType);

            StandardIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            bool isStandard = (StandardIn != null) && paramInfo.Remove(StandardIn);

            StandardOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));
            isStandard &= (StandardOut != null) && paramInfo.Remove(StandardOut) && paramInfo.Count == 0;

            IsStandard = isStandard;

            if (IsStandard)
            {
                Action = ReflectionActionStandard();
                return true;
            }

            if (IsAsync && Method.ReturnParameter.ParameterType.IsGenericType)
            {

            }

            ParamIn = Parameters.Where((p) => ParamAttributes<PayloadInAttribute>(p)).FirstOrDefault();
            TypeIn = ParamIn.ParameterType;

            ParamOut = Parameters.Where((p) => ParamAttributes<PayloadOutAttribute>(p)).FirstOrDefault();

            if (ParamOut == null && ParamAttributes<PayloadOutAttribute>(Method.ReturnParameter))
            {
                ParamOut = Method.ReturnParameter;
                IsReturnValue = true;
            }
            else if (ParamOut != null && !ParamAttributes<OutAttribute>(ParamOut))
            {

            }



            return false;
        }

        private bool ParamAttributes<A>(ParameterInfo info)
            where A: Attribute
        {
            return Attribute.GetCustomAttribute(info, typeof(A)) != null;
        }

        public ParameterInfo StandardIn { get; private set; }

        public ParameterInfo StandardOut { get; private set; }

        public ParameterInfo ParamIn { get; private set; }

        public Type TypeIn { get; set; }

        public ParameterInfo ParamOut { get; private set; }

        public Type TypeOut { get; set; }

        #region Reference(CommandContractAttribute attr)
        /// <summary>
        /// This is the command reference.
        /// </summary>
        /// <param name="attr">The contract attribute.</param>
        /// <returns>The reference id.</returns>
        public string Reference(CommandContractAttribute attr)
        {
            return $"{Method.Name}/{attr.Header.ToKey()}";
        } 
        #endregion

        /// <summary>
        /// This is the command action that is executed.
        /// </summary>
        public Func<TransmissionPayload, List<TransmissionPayload>, Task> Action{get; protected set;}

        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandard()
        {
            if (IsAsync)
                return async (pIn, pOut) =>
                {
                    await (Task)Method.Invoke(Command, new object[] { pIn, pOut });
                };
            else
                return async (pIn, pOut) =>
                {
                    Method.Invoke(Command, new object[] { pIn, pOut });
                };
        }

        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandardParameter(Type paramInType)
        {
            return async (pIn, pOut) =>
            {
                await (Task)Method.Invoke(Command, new object[] { pIn, pOut });
            };
        }
    }
}
