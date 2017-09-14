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

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xigadee
{
    /// <summary>
    /// This class is used to provide helper methods to the harness.
    /// </summary>
    public static partial class CommandHarnessHelper
    {
        /// <summary>
        /// This class is used to create the command using reflection to pass in the policy when provided.
        /// </summary>
        /// <typeparam name="C">The command type.</typeparam>
        /// <typeparam name="P">The policy type.</typeparam>
        /// <param name="policy">The policy.</param>
        /// <returns>Returns a function that can be used to create the command with the policy specified.</returns>
        public static Func<C> DefaultCreator<C,P>(P policy = null)
            where C: class, ICommand
            where P : CommandPolicy, new()
        {
            if (policy == null)
                return ServiceHarnessHelper.DefaultCreator<C>();

            var constructors = typeof(C).GetConstructors();

            //OK, special case when a constructor is not actually set, but inherits from its parent.
            if (constructors.Length == 1)
            {
                var cParams = constructors[0].GetParameters();

                if (cParams.Length == 0)
                    return ServiceHarnessHelper.DefaultCreator<C>();
            }

            //Get the first constructor that supports all optional parameters (i.e. parameterless) with the least amount of parameters.
            var constructor = constructors
                .Where((c) => c.GetParameters().All((p) => p.IsOptional))
                .Where((c) => c.GetParameters().Any((p) => p.ParameterType == typeof(P)))
                .FirstOrDefault();

            if (constructor == null)
                throw new CommandHarnessInvalidConstructorException($"{typeof(C)} Command does not have a supported constructor.");

            //Create an array of all Type.Missing for each of the optional parameters.
            var parameters = constructor.GetParameters()
                .Select((p) => {
                    if (p.ParameterType == typeof(P))
                        return policy;
                    else
                        return Type.Missing;
                    })
                .ToArray();

            return () => (C)Activator.CreateInstance(typeof(C)
                    , BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding
                    , null
                    , parameters
                    , CultureInfo.CurrentCulture
                    );
        }
    }
}
