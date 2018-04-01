using System;

namespace Xigadee
{
    /// <summary>
    /// This static class provides bit mapping functionality for primitive types.
    /// </summary>
    public static class BitHelper
    {
        #region BitErrCheck(byte bit, byte check)
        private static void BitErrCheck(byte bit, byte check)
        {
            if (bit < 0 || bit >= check)
                throw new ArgumentOutOfRangeException("bit", "The bit parameter is out of range.");
        }
        #endregion

        #region BitReverse(this int data)
        /// <summary>
        /// This method reverses the hashcode so that it is ordered in reverse based on bit value, i.e.
        /// xxx1011 => 1101xxxx => Bucket 1 1xxxxx => Bucket 3 11xxxxx => Bucket 6 110xxx etc.
        /// </summary>
        /// <param name="data">The data to reverse></param>
        /// <returns>Returns the reversed data</returns>
        public static int BitReverse(this int data)
        {
            return data.BitReverse(0x40000000);
        }
        /// <summary>
        /// This method reverses the hashcode so that it is ordered in reverse based on bit value, i.e.
        /// xxx1011 => 1101xxxx => Bucket 1 1xxxxx => Bucket 3 11xxxxx => Bucket 6 110xxx etc.
        /// </summary>
        /// <param name="data">The data to reverse></param>
        /// <param name="hiMask">This is the mask bit to start the reverse process.</param>
        /// <returns>Returns the reversed data</returns>
        public static int BitReverse(this int data, int hiMask)
        {
            int result = 0;

            for (; data > 0; data >>= 1)
            {
                if ((data & 1) > 0)
                    result |= hiMask;

                hiMask >>= 1;

                if (hiMask == 0)
                    break;
            }

            return result;
        }
        #endregion

        #region SByte
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static sbyte BitSet(this sbyte value, byte bit)
        {
            BitErrCheck(bit, 7);
            return (sbyte)(value | ((sbyte)1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static sbyte BitUnset(this sbyte value, byte bit)
        {
            BitErrCheck(bit, 7);
            return (sbyte)(value ^ ((sbyte)1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this sbyte value, byte bit)
        {
            BitErrCheck(bit, 7);
            return (value & ((sbyte)1 << bit)) > 0;
        }
        #endregion
        #region Byte
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static byte BitSet(this byte value, byte bit)
        {
            BitErrCheck(bit, 8);
            return (byte)(value | (1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static byte BitUnset(this byte value, byte bit)
        {
            BitErrCheck(bit, 8);
            return (byte)(value ^ (1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this byte value, byte bit)
        {
            BitErrCheck(bit, 8);
            return (value & (1 << bit)) > 0;
        }
        #endregion

        #region Int16
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static short BitSet(this short value, byte bit)
        {
            BitErrCheck(bit, 15);
            return (short)(value | (short)(1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static short BitUnset(this short value, byte bit)
        {
            BitErrCheck(bit, 15);
            return (short)(value ^ (short)(1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this short value, byte bit)
        {
            BitErrCheck(bit, 15);
            return (value & (1 << bit)) > 0;
        }
        #endregion
        #region UInt16
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static ushort BitSet(this ushort value, byte bit)
        {
            BitErrCheck(bit, 16);
            return (ushort)(value | ((ushort)1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static ushort BitUnset(this ushort value, byte bit)
        {
            BitErrCheck(bit, 16);
            return (ushort)(value ^ ((ushort)1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this ushort value, byte bit)
        {
            BitErrCheck(bit, 16);
            return (value & ((ushort)1 << bit)) > 0;
        }
        #endregion

        #region Int32
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static int BitSet(this int value, byte bit)
        {
            BitErrCheck(bit, 31);
            return value | (1 << bit);
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static int BitUnset(this int value, byte bit)
        {
            BitErrCheck(bit, 31);
            return value ^ (1 << bit);
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this int value, byte bit)
        {
            BitErrCheck(bit, 31);
            return (value & (1 << bit)) > 0;
        }
        #endregion
        #region UInt32
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static uint BitSet(this uint value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (uint)(value | ((uint)1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static uint BitUnset(this uint value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (uint)(value ^ ((uint)1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this uint value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (value & ((uint)1 << bit)) > 0;
        }
        #endregion

        #region Int64
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static long BitSet(this long value, byte bit)
        {
            BitErrCheck(bit, 47);
            return value | ((long)1 << bit);
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static long BitUnset(this long value, byte bit)
        {
            BitErrCheck(bit, 47);
            return value ^ ((long)1 << bit);
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this long value, byte bit)
        {
            BitErrCheck(bit, 47);
            return (value & ((long)1 << bit)) > 0;
        }
        #endregion
        #region UInt64
        /// <summary>
        /// This method sets the bit at the specified value.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static ulong BitSet(this ulong value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (ulong)(value | ((ulong)1 << bit));
        }
        /// <summary>
        /// This method sets the bit at the specified position to 0.
        /// </summary>
        /// <param name="value">The data.</param>
        /// <param name="bit">The bit position.</param>
        /// <returns>Returns the new data.</returns>
        public static ulong BitUnset(this ulong value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (ulong)(value ^ ((ulong)1 << bit));
        }
        /// <summary>
        /// This method checks whether the bit is set.
        /// </summary>
        /// <param name="value">The data to check.</param>
        /// <param name="bit">The bit to check.</param>
        /// <returns>Returns true if the bit is set.</returns>
        public static bool BitCheck(this ulong value, byte bit)
        {
            BitErrCheck(bit, 32);
            return (value & ((ulong)1 << bit)) > 0;
        }
        #endregion

        #region SplitOnMostSignificantBit(int index, int msbStartPosition, out int msbPosition, out int remainder)
        /// <summary>
        /// This method splits the index on both parts.
        /// </summary>
        /// <param name="index">The index to split.</param>
        /// <param name="msbStartPosition">The start bit position to begin searching.</param>
        /// <param name="msbPosition">The highest significant bit position.</param>
        /// <param name="remainder">The remainder minus the 2 to the power of the bit position./</param>
        public static void SplitOnMostSignificantBit(int index, int msbStartPosition, 
            out int msbPosition, out int remainder)
        {
            int mask = (1 << msbStartPosition);
            for (msbPosition = msbStartPosition; msbPosition > 0 && ((index & mask) == 0); msbPosition--)
                mask >>= 1;

            mask--;
            remainder = index & mask;
        }
        #endregion
        #region FindMostSignificantBit(int index, int msbStartPosition)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="msbStartPosition"></param>
        /// <returns></returns>
        public static int FindMostSignificantBit(int index, int msbStartPosition)
        {
            int mask = (1 << msbStartPosition);
            for (; msbStartPosition > 0 && ((index & mask) == 0); msbStartPosition--)
            {
                mask >>= 1;
            }

            return msbStartPosition;
        }
        #endregion
        #region FindMostSignificantBit(int index, int msbStartPosition, out int mask)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="msbStartPosition"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static int FindMostSignificantBit(int index, int msbStartPosition, out int mask)
        {
            mask = (1 << msbStartPosition);
            for (; msbStartPosition > 0 && ((index & mask) == 0); msbStartPosition--)
            {
                mask >>= 1;
            }

            mask--;
            return msbStartPosition;
        }
        #endregion
    }
}
