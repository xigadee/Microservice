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
using System.Xml.Linq;

namespace Xigadee
{
    /// <summary>
    /// This is class that holds the options entry on the console menu
    /// </summary>
    public class ConsoleSwitchOption: ConsoleOption
    {

        #region Constructors
        public ConsoleSwitchOption(
              string textSwitchedOff, Func<ConsoleMenu, ConsoleSwitchOption, bool> switchOn
            , string textSwitchedOn, Func<ConsoleMenu, ConsoleSwitchOption, bool> switchOff
            , ConsoleMenu childMenu = null
            , Func<ConsoleMenu, ConsoleSwitchOption, bool> switched = null
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , string shortcut = null
            )
            : base(textSwitchedOff, null, childMenu, enabled, display, null, shortcut)
        {
            Action = (m,o) =>
            {
                ConsoleSwitchOption to = o as ConsoleSwitchOption;

                if (IsSwitchedOn)
                    IsSwitchedOn = !switchOff(m,to);
                else
                    IsSwitchedOn = switchOn(m,to);

                if (switched != null)
                {
                    if (switched(m, to))
                    {
                    }
                }

                Text = IsSwitchedOn? textSwitchedOn : textSwitchedOff;
            };
        }
        #endregion

        /// <summary>
        /// This boolean switch identifies whether the option has been enabled.
        /// </summary>
        public bool IsSwitchedOn { get; set; }


    }
}
