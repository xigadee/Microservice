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
    public class ConsoleOption
    {
        #region Constructors
        public ConsoleOption(string text
            , Action<ConsoleMenu, ConsoleOption> action
            , ConsoleMenu childMenu = null
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            , string shortcut = null
            )
        {
            Action = action;
            Menu = childMenu;
            Text = text;
            FnDisplay = display;
            FnEnabled = enabled ?? ((m, o) => true);
            FnSelected = selected;
            Shortcut = shortcut;
        }

        public ConsoleOption(string text
            , ConsoleMenu childMenu
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            , string shortcut = null
            )
            : this(text, null, childMenu, enabled, display, selected, shortcut)
        {
        }

        protected ConsoleOption(string text
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            , string shortcut = null
            )
            : this(text, null, null, enabled, display, selected, shortcut)
        {
        }
        #endregion
        /// <summary>
        /// This is the shorcut action value. This can be used to send simulated key strokes.
        /// </summary>
        public string Shortcut { get; set; }

        /// <summary>
        /// This is the text displayed in the menu option.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// This is the parent menu option.
        /// </summary>
        public ConsoleMenu Menu { get; set; }
        /// <summary>
        /// This function is used to specify when the option is marked as enabled.
        /// </summary>
        public Func<ConsoleMenu, ConsoleOption, bool> FnEnabled { get; set; }
        /// <summary>
        /// This function is called when the option is selected by the user.
        /// </summary>
        public Func<ConsoleMenu, ConsoleOption, bool> FnSelected { get; set; }
        /// <summary>
        /// This function is used to set the display text for the option.
        /// </summary>
        public Func<ConsoleMenu, ConsoleOption, string> FnDisplay { get; set; }
        /// <summary>
        /// This is the action executed when the option is selected.
        /// </summary>
        public Action<ConsoleMenu, ConsoleOption> Action { get; set; }

    }
}
