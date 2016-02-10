using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xigadee
{
    public class ConsoleOption
    {

        #region Constructors
        public ConsoleOption(string text
            , Action<ConsoleMenu, ConsoleOption> action
            , ConsoleMenu childMenu = null
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            )
            : this(text, childMenu, enabled, display)
        {
            Action = action;
        }

        public ConsoleOption(string text
            , ConsoleMenu childMenu
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            )
            : this(text, enabled, display)
        {
            Menu = childMenu;
            IsChildMenu = childMenu != null;
        }

        protected ConsoleOption(string text
            , Func<ConsoleMenu, ConsoleOption, bool> enabled = null
            , Func<ConsoleMenu, ConsoleOption, string> display = null
            )
        {
            Text = text;
            FnDisplay = display;
            FnEnabled = enabled ?? ((m, o) => true);
        } 
        #endregion

        public string Text { get; set; }

        public bool IsChildMenu { get; set; }

        public Func<ConsoleMenu, ConsoleOption, bool> FnEnabled { get; set; }

        public Func<ConsoleMenu, ConsoleOption, string> FnDisplay { get; set; }

        public Action<ConsoleMenu, ConsoleOption> Action { get; set; }

        public ConsoleMenu Menu { get; set; }

    }
}
