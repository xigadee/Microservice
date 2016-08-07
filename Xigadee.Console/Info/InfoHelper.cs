using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class InfoHelper
    {

        #region DisplayInfoMessages()
        /// <summary>
        /// This method displays the info messages on the console.
        /// </summary>
        public static void DisplaySummary(this ConsoleInfoContext context, int count = 2)
        {
            if (context.InfoMessages.Count > 0)
            {
                int start = 0;
                var infoArray = context.InfoMessages.OrderByDescending((i) => i.Priority).ToList();

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
            }

            while (count-- >0)
                System.Console.WriteLine();
        }
        #endregion

    }
}
