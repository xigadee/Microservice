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
            var paramInfo = info.GetParameters().ToList();
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

            //if (paramInfo.Count() == 0)
            //    return;

            bool isGeneric = true;
            var genericIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            if (genericIn != null)
                isGeneric &= paramInfo.Remove(genericIn);
            var genericOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));
            if (genericOut != null)
                isGeneric &= paramInfo.Remove(genericOut) && paramInfo.Count == 0;


            if (isGeneric && RegisterGenericSignature(commandAttrs, info, genericIn, genericOut))
                return;

            //No, OK .. lets proceed, we have a custom signature

            var rqAttr = Attribute.GetCustomAttributes(paramInfo[0]);
            var rsAttr = info.ReturnTypeCustomAttributes?.GetCustomAttributes(typeof(PayloadOutAttribute), true);
        }

        protected virtual bool RegisterGenericSignature(List<CommandContractAttribute> commandAttrs, 
            MethodInfo info, ParameterInfo genericIn, ParameterInfo genericOut)
        {
            //Is this the standard command - async Task Something(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
            if (genericIn != null && genericOut != null && info.ReturnType == typeof(Task))
            {
                commandAttrs.ForEach((a) => 
                    CommandRegister(HeaderFixChannel(a.Header), ReflectionActionStandard(info), referenceId: $"{info.DeclaringType.Name}/{info.Name}")
                    );

                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// This method replaces the channel if the value is null.
        /// </summary>
        /// <param name="header">The incoming header to check.</param>
        /// <param name="channelId">The channel id. When null the base command ChannelId will be used instead.</param>
        /// <returns>Returns a message filter wrapper for the header.</returns>
        protected MessageFilterWrapper HeaderFixChannel(ServiceMessageHeader header, string channelId = null)
        {
            if (header.ChannelId == null)
                header = new ServiceMessageHeader(channelId ?? ChannelId, header.MessageType, header.ActionType);

            return new MessageFilterWrapper(header);
        }


        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandard(MethodInfo info)
        {
            return async (pIn, pOut) =>
            {
                await (Task)info.Invoke(this, new object[] { pIn, pOut });
            };
        }

        private Func<TransmissionPayload, List<TransmissionPayload>, Task> ReflectionActionStandardParameter(MethodInfo info, Type paramInType)
        {
            return async (pIn, pOut) =>
            {
                await (Task)info.Invoke(this, new object[] { pIn, pOut });
            };
        }
    }
}
