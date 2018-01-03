using System;
using System.Collections.Generic;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the base console menu class.
    /// </summary>
    public partial class ConsoleMenu
    {
        /// <summary>
        /// This event is fired when the escape key is pressed on the menu causing it to exit the screen.
        /// </summary>
        public event EventHandler<EventArgs> OnClose;

        /// <summary>
        /// This property contains any custom information for the menu.
        /// </summary>
        public Dictionary<string, ConsoleInfoContext> CustomInfo { get; set; }

        #region Constructor
        /// <summary>
        /// This is the main constructor for the menu.
        /// </summary>
        /// <param name="title">The page title.</param>
        /// <param name="options">The child options for the menu.</param>
        public ConsoleMenu(
              string title
            , params ConsoleOption[] options
            )
        {
            Context = new ConsoleMenuContext(options);
            ContextInfo = new ConsoleInfoContext();

            Context.Title = title;
            Context.ConsoleTitle = title;
            Context.Subtitle = "Please select from the following options";

            Context.EscapeText = "Press escape to exit";
            Context.EscapeWrapper = true;

            Context.Indent1 = 3;
            Context.Indent2 = 6;

            CustomInfo = new Dictionary<string, ConsoleInfoContext>();
        }
        #endregion
        /// <summary>
        /// This property specifies whether the display should use the default context.
        /// </summary>
        public bool ContextInfoInherit { get; set; } = true;
        /// <summary>
        /// This is the menu context that holds the current state.
        /// </summary>
        public ConsoleMenuContext Context { get; set; }
        /// <summary>
        /// This context holds the context info.
        /// </summary>
        public ConsoleInfoContext ContextInfo { get; set; }

        #region Refresh()
        /// <summary>
        /// This method call be called to force a refresh of the display.
        /// </summary>
        public virtual void Refresh()
        {
            ContextInfo.Refresh = true;
        }
        #endregion

        #region Show(object state = null, int pageLength = 9, string shortcut = null)
        /// <summary>
        /// This method displays the menu on the console application.
        /// </summary>
        /// <param name="state">The optional state.</param>
        /// <param name="pageLength">This is the number of menu items per page. The default is 9.</param>
        /// <param name="shortcut">This property allows a supported shortcut to the executed automatically</param>
        /// <param name="contextInfo">This method allows the menu context to be inherited by a child menu. If ContextInfoInherit is set to false, this is ignored.</param>
        public virtual void Show(object state = null, int pageLength = 9, string shortcut = null, ConsoleInfoContext contextInfo = null)
        {
            if (contextInfo != null && ContextInfoInherit)
                ContextInfo = contextInfo;

            Context.State = state;
            Context.PageCurrent = 1;
            Context.PageSet(pageLength);

            if (Context.ConsoleTitle != null)
                System.Console.Title = Context.ConsoleTitle;

            //Execute any registered shortcut
            if (shortcut != null)
                ShortcutInvoke(Context, shortcut);

            Display();

            OnClose?.Invoke(this, null);
        }
        #endregion

        #region ShortcutInvoke ...
        /// <summary>
        /// This method traverses the menu structure and executes the shortcut option.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="shortcut">The shortcut id.</param>
        protected void ShortcutInvoke(ConsoleMenuContext context, string shortcut)
        {
            if (context == null || string.IsNullOrEmpty(shortcut))
                return;

            context.Options?.ForEach((o) => ShortcutInvoke(o, shortcut));
        }
        /// <summary>
        /// This method traverses the menu structure and executes the shortcut option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="shortcut">The shortcut id to execute.</param>
        protected void ShortcutInvoke(ConsoleOption option, string shortcut)
        {
            if (option == null || string.IsNullOrEmpty(shortcut))
                return;

            if (option.Shortcut == shortcut)
            {
                ContextInfo.Add($"Shortcut '{shortcut}' executed.");
                option.Action?.Invoke(this, option);

                return;
            }

            ShortcutInvoke(option.Menu?.Context, shortcut);
        } 
        #endregion

        #region ProcessKeyPress(string shortcut)
        /// <summary>
        /// This method processes a key press and returns when the screen requires refreshing
        /// </summary>
        /// <returns>Returns true if the escape key has been pressed.</returns>
        private bool ProcessKeyPress()
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            //Wait for a key press of a refresh flag to be set
            while (true)
            {
                if (Console.KeyAvailable == true)
                {
                    key = System.Console.ReadKey(true);

                    if (ProcessKey(key))
                        break;
                }
                else if (ContextInfo != null && ContextInfo.Refresh)
                {
                    key = new ConsoleKeyInfo();
                    ContextInfo.Refresh = false;
                    break;
                }

                Thread.Sleep(100);
            }

            return key.Key != ConsoleKey.Escape;
        }
        #endregion
        #region ProcessKey(ConsoleKeyInfo key)
        /// <summary>
        /// This method processes the logic for the individual key press.
        /// </summary>
        /// <param name="key">The console key that has been pressed.</param>
        /// <returns>Returns true if the key press was valid and requires a screen refresh.</returns>
        private bool ProcessKey(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Escape)
                return true;

            if (key.Key == ConsoleKey.LeftArrow)
                return Context.PageDecrement();

            if (key.Key == ConsoleKey.RightArrow)
                return Context.PageIncrement();

            if (ContextInfo != null)
            {
                if (key.Key == ConsoleKey.UpArrow)
                    return ContextInfo.InfoIncrement();

                if (key.Key == ConsoleKey.DownArrow)
                    return ContextInfo.InfoDecrement();
            }

            //Keys 1 - 9
            if (key.KeyChar >= 48 && key.KeyChar <= 57)
            {
                int optionVal = (int)key.KeyChar - 48;

                var option = Context.PageOptionSelect(optionVal);

                if (option != null && option.FnEnabled(this, option))
                {
                    OptionAction(option);
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Display()
        /// <summary>
        /// This method is used to display the menu.
        /// </summary>
        protected virtual void Display()
        {
            do
            {
                DisplayHeader();

                DisplayOptions();

                ContextInfo?.DisplaySummary();

                DisplayFooter();
            }
            while (ProcessKeyPress());
        } 
        #endregion

        #region DisplayHeader()
        /// <summary>
        /// This method displays the header on the console.
        /// </summary>
        private void DisplayHeader()
        {
            ConsoleHelper.Header(Context.Title);

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine();
            System.Console.Write(new string(' ', Context.Indent1));
            System.Console.WriteLine("{0}:", Context.Subtitle);
            System.Console.WriteLine();
        }
        #endregion
        #region DisplayOptions()
        /// <summary>
        /// This method displays the options on the console page.
        /// </summary>
        /// <returns>Returns a list of the disabled options on the page based on their position.</returns>
        private void DisplayOptions()
        {
            int pos = 1;
            for (; pos <= Context.PageOptionsLength; pos++)
            {
                var option = Context.PageOptionSelect(pos);
                if (option == null)
                    break;

                if (option.FnEnabled(this, option))
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;

                System.Console.Write(new string(' ', Context.Indent2));
                string display = option.FnDisplay == null ? option.Text : option.FnDisplay(this, option);

                if (option.FnSelected != null)
                    display = $"[{(option.FnSelected(this, option) ? "x" : " ")}] " + display;

                System.Console.WriteLine("{0}. {1}", pos, display);
            }

            while (pos <= Context.PageOptionsLength)
            {
                System.Console.WriteLine();
                pos++;
            }
        }
        #endregion
        #region DisplayFooter()
        /// <summary>
        /// This methdo displays the footer for the console.
        /// </summary>
        /// <param name="page">The curent page.</param>
        /// <param name="pageMax">The maximum number of pages.</param>
        private void DisplayFooter()
        {
            System.Console.ResetColor();
            System.Console.ForegroundColor = ConsoleColor.Yellow;

            if (Context.PageMax > 1)
            {
                ConsoleHelper.HeaderBar(string.Format("{2} Page {0} of {1} {3}"
                    , Context.PageCurrent
                    , Context.PageMax
                    , Context.PageCurrent > 1 ? "<" : "-"
                    , Context.PageCurrent < Context.PageMax ? ">" : "-"
                    ), character: ' ');
            }
            else
                System.Console.WriteLine();

            if (!string.IsNullOrWhiteSpace(Context.EscapeText))
            {
                if (Context.EscapeWrapper) ConsoleHelper.HeaderBar();
                ConsoleHelper.HeaderBar(Context.EscapeText, character: ' ');
                if (Context.EscapeWrapper) ConsoleHelper.HeaderBar();
            }

            System.Console.ResetColor();
        }
        #endregion

        #region OptionAction(ConsoleOption option)
        /// <summary>
        /// This method executes the action or selects the menu option.
        /// </summary>
        /// <param name="option">The option to execute.</param>
        protected virtual void OptionAction(ConsoleOption option)
        {
            option.Action?.Invoke(this, option);

            if (option.Menu != null)
                option.Menu.Show(Context.State, Context.PageOptionsLength, contextInfo:ContextInfo);
        } 
        #endregion
    }
}
