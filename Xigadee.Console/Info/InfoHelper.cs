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

                int end = start + count;

                if (end > infoArray.Count)
                    end = infoArray.Count;

                for (int i = start; i < end; i++)
                {
                    bool showUp = start > 0;
                    bool showDown = infoArray.Count > start + count;

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
