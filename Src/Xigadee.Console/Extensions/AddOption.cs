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
    public static partial class ConsoleExtensionMethods
    {
        /// <summary>
        /// This method can be called by an external process to update the info messages displayed in the menu.
        /// </summary>
        /// <param name="message">The info message</param>
        /// <param name="refresh">The refresh option flag.</param>
        /// <param name="type">The log type.</param>
        public static ConsoleMenu AddOption(this ConsoleMenu menu, ConsoleOption option)
        {
            menu.Context.Options.Add(option);

            return menu;
        }

        /// <summary>
        /// This method can be called by an external process to update the info messages displayed in the menu.
        /// </summary>
        /// <param name="message">The info message</param>
        /// <param name="refresh">The refresh option flag.</param>
        /// <param name="type">The log type.</param>
        public static ConsoleMenu AddOption(this ConsoleMenu menu, string text
            , Action<ConsoleMenu, ConsoleOption> action
            , ConsoleMenu childMenu = null
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            , string shortcut = null)
        {
            menu.Context.Options.Add(new Xigadee.ConsoleOption(text, action, childMenu, enabled, display, selected, shortcut));

            return menu;
        }
    }
}
