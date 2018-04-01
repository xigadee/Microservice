using System;

namespace Xigadee
{
    /// <summary>
    /// This struct sets the appropriate bits.
    /// </summary>
    public struct BitMaskInt32
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitMaskInt32"/> struct.
        /// </summary>
        /// <param name="bitPosition">The bit position: 0-22.</param>
        /// <exception cref="ArgumentOutOfRangeException">Bit position is out of range. Valid values are 0-22</exception>
        public BitMaskInt32(int bitPosition)
        {
            if (bitPosition < 0 || bitPosition >= 23)
                throw new ArgumentOutOfRangeException("", "Bit position is out of range. Valid values are 0-22");

            BitPosition = bitPosition;
            BitMask = 1 << bitPosition;
            BitMaskNeg = 0x7FFFFFFF ^ BitMask;
        }

        /// <summary>
        /// Gets the bit position.
        /// </summary>
        public int BitPosition { get; }
        /// <summary>
        /// Gets the bit mask.
        /// </summary>
        public int BitMask { get; }
        /// <summary>
        /// Gets the bit mask negative map.
        /// </summary>
        public int BitMaskNeg { get; }

        /// <summary>
        /// Returns true if the bit is set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if set.</returns>
        public bool BitCheck(int value)
        {
            return (value & BitMask) > 0;
        }
        /// <summary>
        /// Applies the bit mask to the incoming data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The adjusted data.</returns>
        public int BitSet(int data)
        {
            return data | BitMask;
        }
        /// <summary>
        /// Removes the bit-mask from the incoming number.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The data with the unset bit.</returns>
        public int BitUnset(int data)
        {
            return data & BitMaskNeg;
        }
    }
}
