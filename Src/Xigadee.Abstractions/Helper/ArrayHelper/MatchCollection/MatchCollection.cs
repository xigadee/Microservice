using System;
using System.Collections;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This structure is used to hold the match terminator collection.
    /// </summary>
    public abstract class MatchCollection<TSource, TMatch> : IEnumerator<MatchTerminator<TSource, TMatch>>
    {
        #region Declarations
        private int position = -1;
        private bool disposed = false;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This protected constructor initializes the collection.
        /// </summary>
        /// <param name="terminators">The terminators enumerator.</param>
        protected MatchCollection(IEnumerator<MatchTerminator<TSource, TMatch>> terminators)
        {
        }
        #endregion // Constrcuctors

        #region Current
        /// <summary>
        /// This property returns the current record.
        /// </summary>
        public virtual MatchTerminator<TSource, TMatch> Current
        {
            get { return this[position]; }
        }
        /// <summary>
        /// This is the default enumerator.
        /// </summary>
        object IEnumerator.Current =>this[position];

        #endregion

        #region Dispose()/Finalize()
        /// <summary>
        /// This method disposes the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// This method disposes of any resources held open by the class.
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                {
                    disposed = true;
                }
        }
        #endregion

        #region MoveNext()
        /// <summary>
        /// This method moves the enumerator to the next position.
        /// </summary>
        /// <returns>Returns true if successful, or false if the end of the collection has been reached.</returns>
        public virtual bool MoveNext()
        {
            return ++position < Count;
        }
        #endregion // MoveNext()

        #region Reset()
        /// <summary>
        /// This method resets the collection to before the first record.
        /// </summary>
        public virtual void Reset()
        {
            position = -1;
        }
        #endregion // Reset()

        #region Position
        /// <summary>
        /// This is the current position in the match collection.
        /// </summary>
        public int Position { get { return position; } }
        #endregion // Position

        #region this[int index]
        /// <summary>
        /// This method returns the specified item for the collection. You should override this indexer.
        /// </summary>
        /// <param name="index">The position index.</param>
        /// <returns></returns>
        public abstract MatchTerminator<TSource, TMatch> this[int index] { get; }
        #endregion

        #region Count
        /// <summary>
        /// This property returns the number of items in the collection. You should override this property.
        /// </summary>
        public abstract int Count { get; }

        #endregion
    }
}
