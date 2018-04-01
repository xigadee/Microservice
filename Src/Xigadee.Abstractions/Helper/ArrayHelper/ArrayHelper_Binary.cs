using System;

namespace Xigadee
{
    public static partial class ArrayHelper
    {
        #region FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart)

        /// <summary>
        /// Finds the character case insensitive.
        /// </summary>
        /// <param name="byData">The data array.</param>
        /// <param name="byteSearch">The byte to search for.</param>
        /// <param name="intStart">The array start position.</param>
        /// <returns>Returns the find position.</returns>
        private static int FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart)
        {
            return FindCharCaseInsensitive(byData,byteSearch, intStart, false);
        }
        #endregion  
        #region FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart, bool blnCaseInsensitive)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byData"></param>
        /// <param name="byteSearch"></param>
        /// <param name="intStart"></param>
        /// <param name="blnCaseInsensitive"></param>
        /// <returns></returns>
        private static int FindCharCaseInsensitive(byte[] byData, byte byteSearch, int intStart, bool blnCaseInsensitive)
        {
            if (!blnCaseInsensitive)
                return Array.IndexOf(byData, (byte)byteSearch, intStart);

            //OK, it's a bit more complex as we have to check for a lower-case char
            byte charLow = 0;
            //Encoding.ASCII.GetBytes(Char.ToLower(Convert.ToChar(bySearch)));
            byte charHigh = 0;
            //Encoding.ASCII.GetBytes(Char.ToUpper(Convert.ToChar(bySearch)));

            int intLower = Array.IndexOf(byData, charLow, intStart, 1);
            int intUpper = Array.IndexOf(byData, charHigh, intStart, 1);

            if (intLower < intUpper && intLower > -1)
            {
                return intLower;
            }
            else
            {
                return intUpper;
            }

        }
        #endregion  

        #region BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byData"></param>
        /// <param name="bySearch"></param>
        /// <param name="intSearchPosition"></param>
        /// <returns></returns>
        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition)
        {
            return byData.BinarySearchExt(bySearch, intSearchPosition, byData.Length, false);
        }
        #endregion  
        #region BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byData"></param>
        /// <param name="bySearch"></param>
        /// <param name="intSearchPosition"></param>
        /// <param name="intLength"></param>
        /// <returns></returns>
        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength)
        {
            return byData.BinarySearchExt(bySearch, intSearchPosition, intLength, false);
        }
        #endregion  
        #region BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength, bool blnIgnoreCase)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byData"></param>
        /// <param name="bySearch"></param>
        /// <param name="intSearchPosition"></param>
        /// <param name="intLength"></param>
        /// <param name="blnIgnoreCase"></param>
        /// <returns></returns>
        public static int BinarySearchExt(this byte[] byData, byte[] bySearch, int intSearchPosition, int intLength, bool blnIgnoreCase)
        {

            if (intSearchPosition < 0)
                throw new ArgumentOutOfRangeException("intSearchPosition", @"Argument must be greater than -1");

            if (intSearchPosition > byData.Length) return -1;

            int intPointer = 0;

            //OK, let's see if we can find the first instance of the first 
            //character of bySearch
            intPointer = FindCharCaseInsensitive(byData,
                bySearch[0], intSearchPosition, blnIgnoreCase);

            //No luck, or has it passed the max length (intLength) that we 
            //should look in?
            if (intPointer < 0 || intPointer > intLength - 1)
                return -1;

            bool blnFailCheck = false;
            while (intPointer >= 0 || intPointer < intLength)
            {
                for (int intLoop = 1; intLoop <= bySearch.Length - 1; intLoop++)
                {
                    if ((intPointer + intLoop) >= intLength)
                    {
                        blnFailCheck = true;
                        break;
                    }

                    if (blnIgnoreCase)
                    {
                        blnFailCheck = blnFailCheck ||
                            (Char.ToLower(Convert.ToChar(bySearch[intLoop])) !=
                            Char.ToLower(Convert.ToChar(byData[intPointer + intLoop])));
                    }
                    else
                    {
                        blnFailCheck = blnFailCheck ||
                            (bySearch[intLoop] != byData[intPointer + intLoop]);
                    }

                    if (blnFailCheck) break;
                }

                if (!blnFailCheck) break;

                //intPointer = Array.IndexOf(byData, bySearch(0), intPointer + 1, intLength - intPointer - 1)
                intPointer = FindCharCaseInsensitive(byData, bySearch[0], intPointer + 1, blnIgnoreCase);

                if (intPointer > -1 || intPointer < intLength)
                    blnFailCheck = false;
            }

            if (blnFailCheck) return -1;

            return intPointer;
        }
        #endregion  
    }
}
