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
    public static partial class ConsoleHelper
    {
        /// <summary>
        /// This class allows switches to be registered against the various config helper classes.
        /// </summary>

        public static void RegisterSwitch<C>(this C config, string id, Action<C, string> setter)
            where C : ConsoleConfigurationBase
        {
            id = id.ToLowerInvariant();
            if (config.Switches.ContainsKey(id))
                setter(config, config.Switches[id]);
        }


        public static bool YesNo(string message, bool appendyesno = true)
        {
            if (appendyesno)
                System.Console.WriteLine("{0} (y/n)", message);
            else
                System.Console.WriteLine("{0}", message);
            ConsoleKeyInfo key;
            do
            {
                key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                    return false;
            }
            while (!(new char[] { 'Y', 'y', 'N', 'n' }).Contains(key.KeyChar));

            return key.KeyChar == 'Y' || key.KeyChar == 'y';
        }

        public static void HeaderBar(string title = null, char character = '#', int? length = null
            , ConsoleColor titleColour = ConsoleColor.White, char? startChar = null, char? endChar = null)
        {
            if (length == null)
                length = System.Console.WindowWidth-2;

            if (title == null || string.IsNullOrWhiteSpace(title))
            {
                System.Console.WriteLine(new string(character, length.Value));
                return;
            }

            if (title.Length > length-2)
                title = title.Substring(0, length.Value-2);

            title = title.Trim();
            int textLength = title.Length;
            

            int lenStart = (length.Value - (textLength + 2)) / 2;
            int lenEnd = lenStart;
            if ((textLength % 2) > 0)
                lenStart++;

            var oldColor = System.Console.ForegroundColor;

            if (startChar.HasValue && lenStart>0)
            {
                System.Console.Write(startChar.Value);
                System.Console.Write(new string(character, lenStart - 1));
            }
            else
                System.Console.Write(new string(character, lenStart));

            System.Console.ForegroundColor = titleColour;
            System.Console.Write(" {0} ", title);
            System.Console.ForegroundColor = oldColor;

            if (endChar.HasValue && lenEnd > 0)
            {
                System.Console.Write(new string(character, lenEnd - 1));
                System.Console.WriteLine(endChar.Value);
            }
            else
                System.Console.WriteLine(new string(character, lenEnd));
        }

        public static void Header(string title, ConsoleColor titleColour = ConsoleColor.White)
        {
            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            HeaderBar();
            HeaderBar(title, character:' ');
            HeaderBar();
            //System.Console.WriteLine();
            //System.Console.ForegroundColor = titleColour;
            //System.Console.WriteLine("   {0}", title);
            System.Console.WriteLine();
        }

        public static int? Options(string title
            , string[] options
            , string subtitle = "Please select from the following options"
            , string escapeText = "Press escape to exit"
            , bool escapeWrapper = false
            , int[] disabled = null
            , int indent1 = 3
            , int indent2 = 6
            , string[] infoMessages = null
            )
        {
            if (disabled == null)
                disabled = new int[]{};

            Header(title);

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine();
            System.Console.Write(new string(' ', indent1));
            System.Console.WriteLine("{0}:", subtitle);
            System.Console.WriteLine();
            for (int i = 1; i <= (int)options.Length; i++)
            {
                if (!disabled.Contains(i-1))
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;

                System.Console.Write(new string(' ', indent2));
                System.Console.WriteLine("{0}. {1}", i, options[i - 1]);
            }

            System.Console.WriteLine();
            if (infoMessages != null)
            {
                infoMessages.ForEach(m => HeaderBar(m, character: ' ', titleColour: ConsoleColor.Gray));           
            }
            System.Console.WriteLine();

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            if (!string.IsNullOrWhiteSpace(escapeText))
            {
                if (escapeWrapper) HeaderBar();
                HeaderBar(escapeText, character:' ');
                if (escapeWrapper) HeaderBar();
            }

            System.Console.ResetColor();

            ConsoleKeyInfo key;
            int? retValue = null;
            do
            {
                key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                    return null;

                retValue = ((int)key.KeyChar) - 49;
            }
            while (retValue.Value < 0 || retValue.Value >= options.Length || disabled.Contains(retValue.Value));

            return retValue;
        }

        #region ReadString(out string password, char? mask = '*', int[] charFilter = null)
        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in </returns>
        public static bool ReadString(out string password, char? mask = '*', int[] charFilter = null)
        {
            if (charFilter == null)
                charFilter = new int[] { 0, 27, 9, 10 /*, 32 space, if you care */ };

            var pass = new Stack<char>();

            bool? result = null;

            while (!result.HasValue)
            {
                ConsoleKeyInfo chr = System.Console.ReadKey(true);

                switch (chr.Key)
                {
                    case ConsoleKey.Enter:
                        result = true;
                        break;
                    case ConsoleKey.Escape:
                        result = false;
                        break;
                    case ConsoleKey.Backspace:
                        if ((chr.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                            while (pass.Count > 0)
                            {
                                System.Console.Write("\b \b");
                                pass.Pop();
                            }
                        else
                            if (pass.Count > 0)
                            {
                                System.Console.Write("\b \b");
                                pass.Pop();
                            }
                        break;
                    default:
                        char val = chr.KeyChar;
                        if (charFilter.Count(x => val == x) > 0) { }
                        else
                        {
                            pass.Push((char)val);
                            if (!mask.HasValue)
                                System.Console.Write(val);
                            else
                                System.Console.Write(mask.Value);
                        }
                        break;
                }
            }

            System.Console.WriteLine();

            if (result.Value)
            {
                password = new string(pass.Reverse().ToArray());
                return true;
            }

            password = null;
            return false;
        }
        #endregion


    }
}
