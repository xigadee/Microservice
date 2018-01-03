using System;

namespace Xigadee
{
    /// <summary>
    /// This is class that holds the options entry on the console menu
    /// </summary>
    public class ConsoleOption
    {
        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOption"/> class.
        /// </summary>
        /// <param name="text">The display text.</param>
        /// <param name="action">The action.</param>
        /// <param name="childMenu">The child menu.</param>
        /// <param name="enabled">The enabled function.</param>
        /// <param name="display">The display function.</param>
        /// <param name="selected">The selected function.</param>
        /// <param name="shortcut">The shortcut identifier.</param>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOption"/> class.
        /// </summary>
        /// <param name="text">The display text.</param>
        /// <param name="childMenu">The child menu.</param>
        /// <param name="enabled">The enabled function.</param>
        /// <param name="display">The display function.</param>
        /// <param name="selected">The selected function.</param>
        /// <param name="shortcut">The shortcut identifier.</param>
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOption" /> class.
        /// </summary>
        /// <param name="text">The display text.</param>
        /// <param name="enabled">The enabled function.</param>
        /// <param name="display">The display function.</param>
        /// <param name="selected">The selected function.</param>
        /// <param name="shortcut">The shortcut identifier.</param>
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
        /// This is the shortcut action value. This can be used to send simulated key strokes.
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
