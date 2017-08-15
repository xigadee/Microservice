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

namespace Xigadee
{
    /// <summary>
    /// This harness is used to test command functionality manually.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public class CommandHarness<C>:ServiceHarness<C, CommandHarnessDependencies<C>>
        where C: class, ICommand
    {
        /// <summary>
        /// Contains the set of active registered commands.
        /// </summary>
        public Dictionary<MessageFilterWrapper, bool> RegisteredCommands { get; } = new Dictionary<MessageFilterWrapper, bool>();
        /// <summary>
        /// Contains the set of active registered schedules.
        /// </summary>
        public Dictionary<CommandJobSchedule, bool> RegisteredSchedules { get; } = new Dictionary<CommandJobSchedule, bool>();

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="creator">This is the creator function to create the command. If the command supports a parameterless constructor, then you can leave this blank.</param>
        public CommandHarness(Func<C> creator = null):base(new CommandHarnessDependencies<C>(creator))
        {
        }

        /// <summary>
        /// This override creates the command.
        /// </summary>
        /// <returns>Returns the command.</returns>
        protected override C Create()
        {
            var command =  Dependencies.Creator();

            command.OnCommandChange += Command_OnCommandChange;
            command.OnScheduleChange += Command_OnScheduleChange;

            return command;
        }



        private void Command_OnScheduleChange(object sender, ScheduleChangeEventArgs e)
        {
            if (e.IsRemoval)
            {
                if (RegisteredSchedules.ContainsKey(e.Schedule))
                    RegisteredSchedules.Remove(e.Schedule);
            }
            else
                RegisteredSchedules.Add(e.Schedule, e.IsMasterJob);
        }

        private void Command_OnCommandChange(object sender, CommandChangeEventArgs e)
        {
            if (e.IsRemoval)
            {
                if (RegisteredCommands.ContainsKey(e.Key))
                    RegisteredCommands.Remove(e.Key);
            }
            else
                RegisteredCommands.Add(e.Key,e.IsMasterJob);
                   
        }
    }
}
