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
            )
        {
            Action = action;
            Menu = childMenu;
            Text = text;
            FnDisplay = display;
            FnEnabled = enabled ?? ((m, o) => true);
            FnSelected = selected;
        }

        public ConsoleOption(string text
            , ConsoleMenu childMenu
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            )
            : this(text, null, childMenu, enabled, display, selected)
        {
        }

        protected ConsoleOption(string text
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            , Func<ConsoleMenu, ConsoleOption, bool> selected = null
            )
            : this(text, null, null, enabled, display, selected)
        {
        }
        #endregion
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
