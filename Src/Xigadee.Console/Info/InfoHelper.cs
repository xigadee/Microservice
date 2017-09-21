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
