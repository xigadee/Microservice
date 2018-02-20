using System;

namespace Xigadee
{
    public static class InfoHelper
    {
        private static int DisplaySummaryMessages(ConsoleInfoContext context, int count)
        {
            int current = 0;

            foreach (var info in context.GetCurrent(count))
            {
                ConsoleColor infoColor;
                switch (info.Type)
                {
                    case LoggingLevel.Error:
                    case LoggingLevel.Fatal:
                        infoColor = ConsoleColor.Red;
                        break;
                    case LoggingLevel.Warning:
                        infoColor = ConsoleColor.Yellow;
                        break;
                    default:
                        infoColor = ConsoleColor.Gray;
                        break;
                }

                current++;
                bool showUp = current == 1 && info.LoggingId<(context.Count -1);
                bool showDown = current == count && info.LoggingId>0;

                ConsoleHelper.HeaderBar(
                    $"{info.LoggingId+1}. {info.Message}"
                    , character: ' '
                    , titleColour: infoColor
                    , startChar: showUp ? (char)8593 : (char?)null
                    , endChar: showDown ? (char)8595 : (char?)null
                    );
            }

            return count - current;
        }

        #region DisplayInfoMessages()
        /// <summary>
        /// This method displays the info messages on the console.
        /// </summary>
        public static void DisplaySummary(this ConsoleInfoContext context, int count = 3)
        {
            if (context.InfoMessages.Count > 0)
                count = DisplaySummaryMessages(context, count);

            while (count-- >0)
                System.Console.WriteLine();
        }
        #endregion

    }
}
