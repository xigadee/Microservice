#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {

        #region CommandsRegisterReflection()
        /// <summary>
        /// This method should be implemented to populate supported commands.
        /// </summary>
        protected virtual void CommandsRegisterReflection()
        {
            GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where((m) => m.CustomAttributes.Count((a) => a.AttributeType == typeof(CommandContractAttribute)) > 0)
                .ForEach((i) => CommandRegisterReflection(i));
        }

        /// <summary>
        /// This method should be implemented to populate supported commands.
        /// </summary>
        protected virtual void CommandRegisterReflection(MethodInfo info)
        {
            //Get the method calling parameters
            var paramInfo = info.GetParameters();
            //And get the return type.
            var returnType = info.ReturnType;

            //Now get the CommandContractAttribute.
            var commandAttrs = Attribute.GetCustomAttributes(info)
                .Where((a) => a is CommandContractAttribute)
                .Cast<CommandContractAttribute>()
                .ToList();
            //This shouldn't happen, but check anyway.
            if (commandAttrs.Count == 0)
                return;
            
            var genericIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            var genericOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));

            //Is this the standard command - async Task Something(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
            if (paramInfo.Length == 2 && genericIn != null && genericOut != null && returnType == typeof(Task))
            {
                commandAttrs.ForEach((a) => CommandsRegisterReflectionStandard(info, a));
                return;
            }

            //No, OK .. lets proceed

            var rqAttr = Attribute.GetCustomAttributes(paramInfo[0]);
            var rsAttr = info.ReturnTypeCustomAttributes?.GetCustomAttributes(typeof(PayloadOutAttribute), true);
        }

        protected virtual bool ValidateStandardCall(MethodInfo info, ParameterInfo[] paramInfo, List<CommandContractAttribute> commandAttrs)
        {
            if (paramInfo.Length != 2)
                return false;

            var genericIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            var genericOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));

            //Is this the standard command - async Task Something(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
            if (genericIn != null && genericOut != null && info.ReturnType == typeof(Task))
            {
                commandAttrs.ForEach((a) => CommandsRegisterReflectionStandard(info, a));
                return true;
            }

            return false;
        }

        protected virtual void CommandsRegisterReflectionStandard(MethodInfo info, CommandContractAttribute attr)
        {
            var header = attr.Header;


            if (header.ChannelId == null)
                header = new Xigadee.ServiceMessageHeader(ChannelId, header.MessageType, header.ActionType);

            var wrapper = new MessageFilterWrapper(header);


            Func<TransmissionPayload, List<TransmissionPayload>, Task> action = async (pIn, pOut) =>
            {
                await (Task)info.Invoke(this, new object[] { pIn, pOut });
            };

            CommandRegister(wrapper, action);

            //This register the standard raw command etc. (TransmissionPayload payloadin, List<TransmissionPayload> payloadsOut)
        }
        #endregion
    }
}
