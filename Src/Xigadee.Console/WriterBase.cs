#region using
using System;
using System.Collections.Concurrent;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This abstract class allows for multiple output formats for the mail output.
    /// </summary>
    public abstract class WriterBase<I>: IDisposable
    {
        #region Declarations
        protected bool mDisposed = false;

        protected ConcurrentQueue<I> mLogQueue;

        protected ManualResetEventSlim mReset;

        protected Func<I, bool> mLogOK;

        protected Thread mThreadLog;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logOK"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WriterBase(Func<I, bool> logOK)
        {
            mLogOK = logOK==null?(i) => true:logOK;
            ResourcesAcquire();
        } 
        #endregion

        #region ResourcesAcquire()
        /// <summary>
        /// This method sets the resources needed for the writer.
        /// </summary>
        protected virtual void ResourcesAcquire()
        {
            mReset = new ManualResetEventSlim(false);
            mLogQueue = new ConcurrentQueue<I>();
            mThreadLog = new Thread(SpinWrite);
            mThreadLog.Start();
        } 
        #endregion

        #region Dispose()
        /// <summary>
        /// This method is called when the logger is disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the concurrent queue.
        /// </summary>
        /// <param name="disposing">Set to true if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            if (disposing)
            {
                mDisposed = true;
                mReset.Set();

                mThreadLog.Join();

                BufferClear();
                ResoursesRelease();
            }
        } 
        #endregion

        #region BufferClear()
        /// <summary>
        /// This method clears the buffer before the class in disposed.
        /// </summary>
        protected virtual void BufferClear()
        {
            WriteBuffer();
        } 
        #endregion

        #region ResoursesRelease()
        /// <summary>
        /// This method releases the resources as part of the dispose process.
        /// </summary>
        protected virtual void ResoursesRelease()
        {
            mReset.Dispose();
            mReset = null;
            mLogQueue = null;
            mThreadLog = null;
        } 
        #endregion

        #region Write(I item, bool filter)
        /// <summary>
        /// This method allows for manual writes to the event log.
        /// </summary>
        /// <param name="item">The message.</param>
        /// <param name="filter">Set this to false to override the filter function.</param>
        public virtual void Write(I item, bool filter)
        {
            if (mDisposed)
                throw new ObjectDisposedException("Write");

            if (!filter || mLogOK(item))
            {
                mLogQueue.Enqueue(item);
                mReset.Set();
            }
        }
        #endregion
        #region Write(object state, I item)
        /// <summary>
        /// This method writes the mail from the handler.
        /// </summary>
        /// <param name="state">This class is ignored but allows EventHandlers to connect directly.</param>
        /// <param name="item">This is the item to be logged.</param>
        public virtual void Write(object state, I item)
        {
            Write(item, true);
        } 
        #endregion

        #region SpinWrite(object state)
        /// <summary>
        /// This method is used to manage logging using a single thread.
        /// </summary>
        /// <param name="state">The logged state.</param>
        protected virtual void SpinWrite(object state)
        {
            while (!mDisposed)
            {
                int count = WriteBuffer();
                mReset.Wait(250);
            }
        } 
        #endregion

        #region WriteBuffer()
        /// <summary>
        /// This method will write the current items in the queue to the stream processor.
        /// </summary>
        protected virtual int WriteBuffer()
        {
            I item;
            int items = 0;
            while (mLogQueue.TryDequeue(out item))
            {
                WriteInternal(item);
                items++;
            }

            return items;
        } 
        #endregion

        /// <summary>
        /// This abstract method should be used to log data to the output format.
        /// </summary>
        /// <param name="item">The item to log.</param>
        protected abstract void WriteInternal(I item);

    }
}
