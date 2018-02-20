using System;

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
