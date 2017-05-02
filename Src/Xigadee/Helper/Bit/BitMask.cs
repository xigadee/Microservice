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

namespace Xigadee
{
    /// <summary>
    /// This struct sets the appropriate bits.
    /// </summary>
    public struct BitMaskInt32
    {
        int bitPosition;
        int bitMask, bitMaskNeg;

        public BitMaskInt32(int bitPosition)
        {
            if (bitPosition < 0 || bitPosition >= 23)
                throw new ArgumentOutOfRangeException("", "Bit position is out of range. Valid values are 0-22");

            this.bitPosition = bitPosition;
            bitMask = 1 << bitPosition;
            bitMaskNeg = 0x7FFFFFFF ^ bitMask;
        }

        public int BitPosition { get { return bitPosition; } }

        public bool BitCheck(int value)
        {
            return (value & bitMask) > 0;
        }

        public int BitSet(int data)
        {
            return data | bitMask;
        }

        public int BitUnset(int data)
        {
            return data & bitMaskNeg;
        }
    }
}
