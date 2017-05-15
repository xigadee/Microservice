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
    /// <summary>
    /// This class contains the match state.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    public class MatchState<TSource>
    {
        #region Declarations
        private const int cnGrowthFactor = 100;
        private TSource[] mData;
        private int mDataPosition;
        private bool mDataCopy;
        #endregion  

        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public MatchState()
        {
            SlidingWindow = new Queue<TSource>();
            MatchPosition = -1;
            Position = 0;
            Length = 0;
            CarryOver = 0;
            Success = false;
            MultipartMatch = false;

            mData = null;
            mDataPosition = -1;
            mDataCopy = false;
        }
        /// <summary>
        /// This constructor is used to inform the match sequence logic to copy the matched data to the data array.
        /// </summary>
        /// <param name="copyData">Set this to true if you wish the matched data to be copied to the array.</param>
        public MatchState(bool copyData):this()
        {
            if (!copyData)
                return;

            mDataCopy = true;
        }
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="Position">The current position.</param>
        /// <param name="CarryOver">The current carry over position.</param>
        public MatchState(int Position, int CarryOver)
            : this()
        {
            this.Position = Position;
            this.CarryOver = CarryOver;
        }
        #endregion

        #region DataCopy
        /// <summary>
        /// This public property identifies whether the data that has been read will be copied to the array.
        /// </summary>
        public bool DataCopy
        {
            get { return mDataCopy; }
        }
        #endregion  
        #region Data
        /// <summary>
        /// This is the data array.
        /// </summary>
        public TSource[] Data
        {
            get { return mData; }
        }
        #endregion  
        #region DataPosition
        /// <summary>
        /// This is the current position within the data array,
        /// </summary>
        public int DataPosition
        {
            get { return mDataPosition; }
            set { mDataPosition = value; }
        }
        #endregion  

        #region DataLog(TSource item)
        /// <summary>
        /// This method saves the data element in the array and moves the position forward.
        /// </summary>
        /// <param name="item">The item to log.</param>
        public void DataLog(TSource item)
        {
            if (!mDataCopy)
                return;

            mDataPosition++;

            if (mData == null)
                mData = new TSource[cnGrowthFactor];
            else if (mData.Length == mDataPosition)
            {
                TSource[] temp = new TSource[cnGrowthFactor + mData.Length];
                Array.Copy(mData, temp, mDataPosition);
                mData = temp;
            }

            mData[mDataPosition] = item;

        }
        #endregion  


        #region SlidingWindow
        /// <summary>
        /// The sliding window queue.
        /// </summary>
        public Queue<TSource> SlidingWindow { get; set; }
        #endregion // SlidingWindow

        #region MatchPosition
        /// <summary>
        /// The match position in the source array
        /// </summary>
        public int MatchPosition { get; set; }
        #endregion // Position
        #region Position
        /// <summary>
        /// The match position in the source array
        /// </summary>
        public int Position { get; set; }
        #endregion // Position
        #region Length
        /// <summary>
        /// The length of the match. This is needed because some matches are of a variable length.
        /// </summary>
        public int Length { get; set; }
        #endregion // Position

        #region CarryOver
        /// <summary>
        /// The number of carry over position in the match array.
        /// </summary>
        public int CarryOver { get; set; }
        #endregion // CarryOver

        #region MultipartMatch
        /// <summary>
        /// The multipart match informs the routine that this is only part of a maultiple match.
        /// </summary>
        public bool MultipartMatch { get; set; }
        #endregion // MultipartMatch

        #region Success
        /// <summary>
        /// Indicates whether the match is a success.
        /// </summary>
        public bool Success { get; set; }
        #endregion // Success
        #region IsTerminator
        /// <summary>
        /// Indicates whether the match is a terminator. This additional functionality is needed for complex matches.
        /// </summary>
        public bool IsTerminator { get; set; }
        #endregion // IsTerminator
        #region IsPartialMatch
        /// <summary>
        /// Indicates whether we are currently processing a partial match.
        /// </summary>
        public bool IsPartialMatch
        {
            get;
            set;
        }
        #endregion // IsPartialMatch
        #region IsMatch
        /// <summary>
        /// Identifies when there is a match.
        /// </summary>
        public bool IsMatch
        {
            get;
            set;
        }
        #endregion // IsMatch

        #region SetMatch(int Position)
        /// <summary>
        /// This method sets the match at the specific position.
        /// </summary>
        /// <param name="Position">The position in the source array.</param>
        public void SetMatch(int Position)
        {
            Success = true;
            IsMatch = true;
            IsPartialMatch = false;
            this.CarryOver = CarryOver;
            this.Position += Position;
        }
        #endregion // SetMatch(int Position)
        #region SetPartialMatch(int Position, int CarryOver)
        /// <summary>
        /// This method sets a partial match.
        /// </summary>
        /// <param name="Position">The position in the source array.</param>
        /// <param name="CarryOver">The carry over position.</param>
        public void SetPartialMatch(int Position, int CarryOver)
        {
            Success = true;
            IsMatch = false;
            IsPartialMatch = true;
            this.CarryOver = CarryOver;
            this.Position += Position;
        }
        #endregion // SetPartialMatch(int Position, int CarryOver)
    }
}
