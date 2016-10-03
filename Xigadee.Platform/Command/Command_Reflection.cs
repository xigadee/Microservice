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
            var paramInfo = info.GetParameters();

            var genericIn = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(TransmissionPayload));
            var genericOut = paramInfo.FirstOrDefault((p) => p.ParameterType == typeof(List<TransmissionPayload>));

            var returnType = info.ReturnType;

            var commandAttrs = Attribute.GetCustomAttributes(info).Where((a) => a is CommandContractAttribute).Cast<CommandContractAttribute>().ToList();
            if (commandAttrs.Count == 0)
                return;

            //Is this the standard command
            if (paramInfo.Length == 2 && genericIn != null && genericOut != null)
            {
                commandAttrs.ForEach((a) => CommandsRegisterReflectionStandard(info, a));
                return;
            }



            var hmm = Attribute.GetCustomAttributes(paramInfo[0]);

        }

        protected virtual void CommandsRegisterReflectionStandard(MethodInfo info, CommandContractAttribute attr)
        {
        }
        #endregion
    }
}
