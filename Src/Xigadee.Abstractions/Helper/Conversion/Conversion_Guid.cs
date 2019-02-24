#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
#endregion
namespace Xigadee
{
    public static partial class ConversionHelper
    {

        #region LuhnCheckGenerate(string code)
        /// <summary>
        /// This method appends the Luhn check digit to the numeric string.
        /// </summary>
        /// <param name="code">The code to append the check digit..</param>
        /// <returns>The code with the check digit appended to the end of the string.</returns>
        public static string LuhnCheckGenerate(this string code)
        {
            var digits = code.ToCharArray().Select((i) => (int)(i - '0')).ToArray();
            var length = digits.Length;

            for (int i = length - 1; i >= 0; i = i - 2)
            {
                digits[i] = digits[i] * 2;
                if (digits[i] > 9)
                    digits[i] = digits[i] - 9;
            }

            var sum = (digits.Sum() * 9).ToString();

            return code + sum.Substring(sum.Length - 1);
        }
        #endregion
        #region LuhnCheckValidate(string data)
        /// <summary>
        /// This method verifies the Luhn check for the numeric string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Returns true if the Luhn check is verified.</returns>
        public static bool LuhnCheckValidate(this string data)
        {
            int sum = 0;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                int add = (data[i] - '0') * (2 - (i + len) % 2);
                add -= add > 9 ? 9 : 0;
                sum += add;
            }
            return sum % 10 == 0;
        }
        #endregion


        /// <summary>
        /// This method converts a Guid to a base36 array, i.e. 0-9&A-Z
        /// </summary>
        /// <param name="id">The incoming Guid</param>
        /// <returns></returns>
        public static string ToBase62(this Guid id)
        {
            return id.ToByteArray().ToAnyBase(Alphabet62);
        }

        /// <summary>
        /// Standard numeric notation
        /// </summary>
        public static readonly char[] Alphabet10 = ("0123456789")
            .ToCharArray();
        /// <summary>
        /// Hexadecimal
        /// </summary>
        public static readonly char[] Alphabet16 = ("0123456789" +
            "abcdef")
            .ToCharArray();
        /// <summary>
        /// Case-insensitive Base 36
        /// </summary>
        public static readonly char[] Alphabet36 = ("0123456789" +
            "abcdefghijklmnopqrstuvwxyz")
            .ToCharArray();
        /// <summary>
        /// Case sensitive base 62
        /// </summary>
        public static readonly char[] Alphabet62 = ("0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            .ToCharArray();


        public static string ToAnyBase(this IEnumerable<byte> toConvert, char[] alphabet, bool bigEndian = false)
        {
            //most .NET-produced byte arrays are "little-endian" (LSB first),
            //but MSB-first is more natural to read bitwise left-to-right;
            //here, we can handle either way.
            var bytes = bigEndian
            ? toConvert.Reverse().ToArray()
            : toConvert.ToArray();
            var builder = new StringBuilder();

            while (bytes.Any(b => b > 0))
            {
                ulong mod;
                bytes = bytes.DivideBy((ulong)alphabet.Length, out mod);
                builder.Insert(0, alphabet[mod]);
            }

            return builder.ToString();
        }

        public static byte[] DivideBy(this byte[] bytes, ulong divisor, out ulong mod, bool preserveSize = true)
        {
            //the byte array MUST be little-endian here or the operation will be totally fubared.
            var bitArray = new BitArray(bytes);

            ulong buffer = 0;
            byte quotientBuffer = 0;
            byte qBufferLen = 0;
            var quotient = new List<byte>();

            //the bitarray indexes its values in little-endian fashion;
            //as the index increases we move from LSB to MSB.
            for (var i = bitArray.Count - 1; i >= 0; --i)
            {
                //The basic idea is similar to decimal long division;
                //starting from the most significant bit, take enough bits
                //to form a number divisible by (greater than) the divisor.
                buffer = (buffer << 1) + (ulong)(bitArray[i] ? 1 : 0);
                if (buffer >= divisor)
                {
                    //Now divide; buffer will never be >= divisor * 2,
                    //so the quotient of buffer / divisor is always 1...
                    quotientBuffer = (byte)((quotientBuffer << 1) + 1);
                    //then subtract the divisor from the buffer, 
                    //to produce the remainder to be carried forward.
                    buffer -= divisor;
                }
                else
                    //to keep our place; if buffer < divisor,
                    //then by definition buffer / divisor == 0 R buffer.
                    quotientBuffer = (byte)(quotientBuffer << 1);


                qBufferLen++;

                if (qBufferLen == 8)
                {
                    //preserveSize forces the output array to be the same number of bytes as the input;
                    //otherwise, insert only if we're inserting a nonzero byte or have already done so,
                    //to truncate leading zeroes.
                    if (preserveSize || quotient.Count > 0 || quotientBuffer > 0)
                        quotient.Add(quotientBuffer);

                    //reset the buffer
                    quotientBuffer = 0;
                    qBufferLen = 0;
                }
            }
            //and when all is said and done what's left in our buffer is the remainder.
            mod = buffer;

            //The quotient list was built MSB-first, but we can't require
            //a little-endian array and then return a big-endian one.
            return quotient.AsEnumerable().Reverse().ToArray();
        }
    }
}
