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
        /// <param name="info">The method to be verified and registered.</param>
        protected virtual void CommandRegisterReflection(MethodInfo info)
        {
            //Load and check the method signature
            var signature = new CommandMethodSignature(this,info);

            CommandRegisterReflection(signature);
        }
        /// <summary>
        /// This method processes the Reflection signature and its attributes.
        /// </summary>
        /// <param name="signature">The signature to process.</param>
        protected virtual void CommandRegisterReflection(CommandMethodSignature signature)
        {
            //Register a command for each of the attributes defined.
            signature.CommandAttributes.ForEach((a) => 
                CommandRegister(CommandChannelAdjust(a), signature.Action, referenceId: signature.Reference(a))
                );
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
                header = new ServiceMessageHeader(ChannelId, header.MessageType, header.ActionType);

            return new MessageFilterWrapper(header);
        }

    }
}
