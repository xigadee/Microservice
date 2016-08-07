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
    public class ConsoleMenu
    {
        public ConsoleMenu(
              string title
            , params ConsoleOption[] options
            )
        {
            Title = title;
            ConsoleTitle = title;
            Subtitle = "Please select from the following options";

            EscapeText = "Press escape to exit";
            EscapeWrapper = true;

            Indent1 = 3;
            Indent2 = 6;

            Options = new List<ConsoleOption>(options);
            InfoMessages = new ConcurrentBag<ErrorInfo>();
        }

        public object State { get; set; }

        public int Indent1 { get; set; }

        public int Indent2 { get; set; }

        public string Title { get; set; }

        public string ConsoleTitle { get; set; }

        public string Subtitle { get; set; }

        public bool EscapeWrapper { get; set; }

        public string EscapeText { get; set; }

        public List<ConsoleOption> Options { get; set; }

        public ConcurrentBag<ErrorInfo> InfoMessages { get; set; }

        /// <summary>
        /// This method displays the options on the console page.
        /// </summary>
        /// <param name="start">The page start.</param>
        /// <param name="pageLength">The page length.</param>
        /// <returns>Returns a list of the disabled options on the page based on their position.</returns>
        private List<int> PaginateOptions(int start, int pageLength)
        {
            List<int> disabled = new List<int>();

            int end = Options.Count - start;
            if (end > pageLength) end = pageLength;

            for (int i = 0; i < end; i++)
            {
                var option = Options[start + i];
                if (option.FnEnabled(this, option))
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                else
                {
                    disabled.Add(i);
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                }

                System.Console.Write(new string(' ', Indent2));
                string display = option.FnDisplay == null? option.Text:option.FnDisplay(this, option);

                if (option.FnSelected != null)
                    display = $"[{(option.FnSelected(this, option)? "x" : " ")}] " + display;

                System.Console.WriteLine("{0}. {1}", i + 1, display);
            }

            return disabled;
        }

        private void DisplayHeader()
        {
            ConsoleHelper.Header(Title);

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine();
            System.Console.Write(new string(' ', Indent1));
            System.Console.WriteLine("{0}:", Subtitle);
            System.Console.WriteLine();
        }

        public void AddInfoMessage(string message, bool refresh = false, EventLogEntryType type = EventLogEntryType.Information)
        {
            InfoMessages.Add(new ErrorInfo(){Type = type, Message = message});

            if (refresh)
                Refresh();
        }

        private int DisplayInfoMessages(int start = 0)
        {
            if (InfoMessages == null || InfoMessages.Count == 0)
                return 0;

            var infoArray = InfoMessages.OrderByDescending((i)=>i.Priority).ToList();

            if (start >= infoArray.Count)
                start = infoArray.Count - 1;

            int end = start + 2;
            if (end > infoArray.Count)
                end = infoArray.Count;

            for (int i = start ; i < end; i++)
            {
                bool showUp = start>0;
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
                    , endChar: showDown && i == end - 1? (char)8595 : (char?)null
                    );
            }
            System.Console.WriteLine();

            return start;
        }

        private void DisplayFooter(int page, int pageMax)
        {
            System.Console.ResetColor();
            System.Console.ForegroundColor = ConsoleColor.Yellow;

            if (pageMax >= 1)
                ConsoleHelper.HeaderBar(string.Format("{2} Page {0} of {1} {3}", page + 1, pageMax + 1
                    , page > 0 ? "<" : "-", page < pageMax ? ">" : "-"), character: ' ');
            else
                System.Console.WriteLine();

            if (!string.IsNullOrWhiteSpace(EscapeText))
            {
                if (EscapeWrapper) ConsoleHelper.HeaderBar();
                ConsoleHelper.HeaderBar(EscapeText, character: ' ');
                if (EscapeWrapper) ConsoleHelper.HeaderBar();
            }

            System.Console.ResetColor();
        }

        private bool mRefresh = false;
        public virtual void Refresh()
        {
            mRefresh = true;
        }

        public virtual void Show(object state, int pageLength = 9, string shortcut = null)
        {
            State = state;

            if (pageLength > 9 || pageLength < 1)
                pageLength = 9;


            ConsoleKeyInfo key;
            int page = 0;
            int pageMax = Options.Count / (pageLength + 1);

            int info = 0;

            do
            {
                int infoMax = InfoMessages.Count;

                if (ConsoleTitle != null)
                    System.Console.Title = ConsoleTitle;

                DisplayHeader();

                var disabled = PaginateOptions(page * pageLength, pageLength);

                System.Console.WriteLine();

                info = DisplayInfoMessages(info);

                DisplayFooter(page, pageMax);

                while (true)
                {
                    if (Console.KeyAvailable == true)
                    {
                        key = System.Console.ReadKey(true);
                        break;
                    }
                    else if (mRefresh)
                    {
                        key = new ConsoleKeyInfo();
                        mRefresh = false;
                        break;
                    }

                    Thread.Sleep(100);
                }
                
                
                if (key.Key == ConsoleKey.Escape)
                    break;

                if (key.Key == ConsoleKey.LeftArrow && page > 0)
                    page--;

                if (key.Key == ConsoleKey.RightArrow && page < pageMax)
                    page++;

                if (key.Key == ConsoleKey.UpArrow && info > 0)
                    info--;

                if (key.Key == ConsoleKey.DownArrow && info < infoMax)
                    info++;

                if (key.KeyChar >= 48 && key.KeyChar <= 57)
                {
                    int optionVal = (int)key.KeyChar - 49;

                    if (disabled.Contains(optionVal))
                        continue;

                    optionVal += page * pageLength;

                    if (optionVal > Options.Count)
                        continue;

                    var option = Options[optionVal];

                    if (option.FnEnabled(this, option) && option.Action != null)
                        option.Action(this, option);

                    if (option.Menu != null)
                        option.Menu.Show(State, pageLength);
                }
            }
            while (true);
        }

    }
}
