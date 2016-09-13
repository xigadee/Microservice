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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ConsoleMenuContext
    {
        public ConsoleMenuContext(ConsoleOption[] options)
        {
            Options = new List<ConsoleOption>(options);
        }

        /// <summary>
        /// This is the list of options for the display.
        /// </summary>
        public List<ConsoleOption> Options { get; }

        public void PageSet(int pageLength)
        {
            if (pageLength > 9 || pageLength < 1)
                pageLength = 9;

            PageOptionsLength = pageLength;

            PageMax = (Options.Count <= PageOptionsLength) ? 1: (Options.Count / PageOptionsLength)+1;
        }

        /// <summary>
        /// This is the number of options per page.
        /// </summary>
        public int PageOptionsLength { get; private set;}

        /// <summary>
        /// This method returns the option for the current page position
        /// </summary>
        /// <param name="pagePosition">1 - maxPage</param>
        /// <returns>Returns the option or null.</returns>
        public ConsoleOption PageOptionSelect(int pagePosition)
        {
            pagePosition += (PageCurrent-1) * PageOptionsLength;

            if (pagePosition > Options.Count)
                return null;

            return Options[pagePosition-1];
        }

        /// <summary>
        /// This is the current page.
        /// </summary>
        public int PageCurrent { get; set; }=1;

        /// <summary>
        /// This is the maximum number of pages.
        /// </summary>
        public int PageMax { get; private set; }=1;

        public bool PageDecrement()
        {
            if (PageCurrent == 1)
                return false;

            PageCurrent--;

            return true;
        }

        public bool PageIncrement()
        {
            if (PageCurrent == PageMax)
                return false;

            PageCurrent++;

            return true;
        }

        public object State { get; set; }

        public int Indent1 { get; set; }

        public int Indent2 { get; set; }

        public string Title { get; set; }

        public string ConsoleTitle { get; set; }

        public string Subtitle { get; set; }

        public bool EscapeWrapper { get; set; }

        public string EscapeText { get; set; }
    }
}
