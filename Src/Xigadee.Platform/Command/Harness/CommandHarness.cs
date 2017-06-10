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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This harness is used to test command functionality manually.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public class CommandHarness<C>:ServiceHarness<C>
        where C: class, ICommand
    {
        Func<C> mCreator;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="creator">This is the creator function to create the command. If the command supports a parameterless constructor, then you can leave this blank.</param>
        public CommandHarness(Func<C> creator = null)
        {
            if (creator == null)
                mCreator = DefaultConstructor();

            mCreator = creator;
        }

        /// <summary>
        /// This override creates the command.
        /// </summary>
        /// <returns>Returns the command.</returns>
        protected override C Create()
        {
            return mCreator();
        }

        /// <summary>
        /// This method checks whether the command supports a parameterless constructor.
        /// </summary>
        /// <returns>Returns the command.</returns>
        private Func<C> DefaultConstructor()
        {
            if (typeof(C).GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentOutOfRangeException($"The command {typeof(C).Name} does not support a parameterless constructor. Please supply a creator function.");

            return () => Activator.CreateInstance<C>();
        }
    }
    
}
