using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base console menu class
    /// </summary>
    public partial class ConsoleMenu
    {
        #region Declarations
        /// <summary>
        /// This is the menu context that holds the current state.
        /// </summary>
        protected ConsoleMenuContext mContextOptions;
        /// <summary>
        /// This context holds the context info.
        /// </summary>
        protected ConsoleInfoContext mContextInfo; 
        #endregion

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
            mContextOptions = new ConsoleMenuContext(options);
            mContextInfo = new ConsoleInfoContext();

            mContextOptions.Title = title;
            mContextOptions.ConsoleTitle = title;
            mContextOptions.Subtitle = "Please select from the following options";

            mContextOptions.EscapeText = "Press escape to exit";
            mContextOptions.EscapeWrapper = true;

            mContextOptions.Indent1 = 3;
            mContextOptions.Indent2 = 6;
        } 
        #endregion

        #region AddInfoMessage(string message, bool refresh = false, EventLogEntryType type = EventLogEntryType.Information)
        /// <summary>
        /// This method can be called by an external process to update the info messages displayed in the menu.
        /// </summary>
        /// <param name="message">The info message</param>
        /// <param name="refresh">The refresh option flag.</param>
        /// <param name="type">The log type.</param>
        public void AddInfoMessage(string message, bool refresh = false, EventLogEntryType type = EventLogEntryType.Information)
        {
            mContextInfo.InfoMessages.Add(new ErrorInfo() { Type = type, Message = message });

            if (refresh)
                Refresh();
        }
        #endregion
        #region Refresh()
        /// <summary>
        /// This method call be called to force a refresh of the display.
        /// </summary>
        public virtual void Refresh()
        {
            mContextInfo.Refresh = true;
        }
        #endregion

        #region Show(object state = null, int pageLength = 9, string shortcut = null)
        /// <summary>
        /// This method displays the menu on the console application.
        /// </summary>
        /// <param name="state">The optional state.</param>
        /// <param name="pageLength"></param>
        /// <param name="shortcut"></param>
        public virtual void Show(object state = null, int pageLength = 9, string shortcut = null, ConsoleInfoContext contextInfo = null)
        {
            if (contextInfo != null)
                mContextInfo = contextInfo;

            mContextOptions.State = state;
            mContextOptions.PageCurrent = 1;
            mContextOptions.PageSet(pageLength);

            if (mContextOptions.ConsoleTitle != null)
                System.Console.Title = mContextOptions.ConsoleTitle;

            //Execute any registered shortcut
            if (shortcut != null)
            {
                var option = mContextOptions.Options.FirstOrDefault((o) => o.Shortcut == shortcut);
                option.Action?.Invoke(this, option);
            }

            do
            {
                DisplayHeader();

                DisplayOptions();

                DisplayInfoMessages();

                DisplayFooter();
            }
            while (ProcessKeyPress());
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
                else if (mContextInfo != null && mContextInfo.Refresh)
                {
                    key = new ConsoleKeyInfo();
                    mContextInfo.Refresh = false;
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
                return mContextOptions.PageDecrement();

            if (key.Key == ConsoleKey.RightArrow)
                return mContextOptions.PageIncrement();

            if (mContextInfo != null)
            {
                if (key.Key == ConsoleKey.UpArrow)
                    return mContextInfo.InfoDecrement();

                if (key.Key == ConsoleKey.DownArrow)
                    return mContextInfo.InfoIncrement();
            }

            //Keys 1 - 9
            if (key.KeyChar >= 48 && key.KeyChar <= 57)
            {
                int optionVal = (int)key.KeyChar - 48;

                var option = mContextOptions.PageOptionSelect(optionVal);

                if (option != null && option.FnEnabled(this, option))
                {
                    OptionAction(option);
                    return true;
                }
            }

            return false;
        } 
        #endregion

        #region DisplayHeader()
        /// <summary>
        /// This method displays the header on the console.
        /// </summary>
        private void DisplayHeader()
        {
            ConsoleHelper.Header(mContextOptions.Title);

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine();
            System.Console.Write(new string(' ', mContextOptions.Indent1));
            System.Console.WriteLine("{0}:", mContextOptions.Subtitle);
            System.Console.WriteLine();
        }
        #endregion
        #region DisplayOptions()
        /// <summary>
        /// This method displays the options on the console page.
        /// </summary>
        /// <param name="start">The page start.</param>
        /// <param name="pageLength">The page length.</param>
        /// <returns>Returns a list of the disabled options on the page based on their position.</returns>
        private void DisplayOptions()
        {
            int pos = 1;
            for (; pos <= mContextOptions.PageOptionsLength; pos++)
            {
                var option = mContextOptions.PageOptionSelect(pos);
                if (option == null)
                    break;

                if (option.FnEnabled(this, option))
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;

                System.Console.Write(new string(' ', mContextOptions.Indent2));
                string display = option.FnDisplay == null ? option.Text : option.FnDisplay(this, option);

                if (option.FnSelected != null)
                    display = $"[{(option.FnSelected(this, option) ? "x" : " ")}] " + display;

                System.Console.WriteLine("{0}. {1}", pos, display);
            }

            while (pos <= mContextOptions.PageOptionsLength)
            {
                System.Console.WriteLine();
                pos++;
            }
        }
        #endregion
        #region DisplayInfoMessages()
        /// <summary>
        /// This method displays the info messages on the console.
        /// </summary>
        private void DisplayInfoMessages()
        {
            if (mContextInfo == null 
                || mContextInfo.InfoMessages.Count == 0)
                return;

            int start = 0;
            var infoArray = mContextInfo.InfoMessages.OrderByDescending((i) => i.Priority).ToList();

            if (start >= infoArray.Count)
                start = infoArray.Count - 1;

            int end = start + 2;

            if (end > infoArray.Count)
                end = infoArray.Count;

            for (int i = start; i < end; i++)
            {
                bool showUp = start > 0;
                bool showDown = infoArray.Count > start + 2;

                ConsoleColor infoColor;
                switch (infoArray[i].Type)
                {
                    case System.Diagnostics.EventLogEntryType.Error:
                        infoColor = ConsoleColor.Red;
                        break;
                    case System.Diagnostics.EventLogEntryType.Warning:
                        infoColor = ConsoleColor.Yellow;
                        break;
                    default:
                        infoColor = ConsoleColor.Gray;
                        break;
                }

                ConsoleHelper.HeaderBar(infoArray[i].Message, character: ' '
                    , titleColour: infoColor
                    , startChar: showUp && i == start ? (char)8593 : (char?)null
                    , endChar: showDown && i == end - 1 ? (char)8595 : (char?)null
                    );
            }

            System.Console.WriteLine();
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

            if (mContextOptions.PageMax > 1)
            {
                ConsoleHelper.HeaderBar(string.Format("{2} Page {0} of {1} {3}"
                    , mContextOptions.PageCurrent
                    , mContextOptions.PageMax
                    , mContextOptions.PageCurrent > 1 ? "<" : "-"
                    , mContextOptions.PageCurrent < mContextOptions.PageMax ? ">" : "-"
                    ), character: ' ');
            }
            else
                System.Console.WriteLine();

            if (!string.IsNullOrWhiteSpace(mContextOptions.EscapeText))
            {
                if (mContextOptions.EscapeWrapper) ConsoleHelper.HeaderBar();
                ConsoleHelper.HeaderBar(mContextOptions.EscapeText, character: ' ');
                if (mContextOptions.EscapeWrapper) ConsoleHelper.HeaderBar();
            }

            System.Console.ResetColor();
        }
        #endregion

        #region OptionAction(ConsoleOption option)
        /// <summary>
        /// This method executes the action or selects the menu option.
        /// </summary>
        /// <param name="option">The option to execute.</param>
        /// <param name="pageLength">The page length.</param>
        protected virtual void OptionAction(ConsoleOption option)
        {
            option.Action?.Invoke(this, option);

            if (option.Menu != null)
                option.Menu.Show(mContextOptions.State, mContextOptions.PageOptionsLength, contextInfo:mContextInfo);
        } 
        #endregion
    }
}
