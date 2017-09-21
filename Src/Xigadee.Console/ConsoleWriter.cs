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

#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This can be used to log to the Console.
    /// </summary>
    public class ConsoleWriter : WriterBase<ErrorInfo>
    {
        #region Class -> Progress
        /// <summary>
        /// This class holds the progress indicator for the console.
        /// </summary>
        public class Progress
        {
            public Progress()
            {
                TickWaitInMs = 100;
                ProgressTickInMs = 1000;

                TickCount = ProgressTickInMs / TickWaitInMs;
            }

            public readonly int TickWaitInMs;

            private readonly int ProgressTickInMs;
            private readonly int TickCount;

            public string ProgressMessageTick = ">";
            public string ProgressMessageStart = "Pending";

            private bool FirstTick = true;
            private bool Active = false;

            private int TickHits;

            private int log = Environment.TickCount;

            public void ConsoleAction()
            {
                if (!Active)
                    return;

                int currentHits = Interlocked.Increment(ref TickHits);

                if ((currentHits % TickCount) > 0)
                    return;

                int current = Environment.TickCount;

                int logOld = Interlocked.Exchange(ref log, current);

                int diff = current - logOld;

                Console.ResetColor();
                if (FirstTick)
                {
                    Console.WriteLine();
                    Console.Write(ProgressMessageStart);
                }
                else
                    Console.Write(ProgressMessageTick);

                FirstTick = false;
            }

            public void Start()
            {
                Active = true;
            }

            public void Stop()
            {
                Active = false;
            }

            public void Reset()
            {
                int old = Interlocked.Exchange(ref TickHits, 0);
                FirstTick = true;
                if (old > TickCount)
                    Console.WriteLine();
            }
        } 
        #endregion

        #region Declarations
        private Progress mProgress;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showProgress"></param>
        public ConsoleWriter(ConsoleWriter.Progress progress = null, Func<ErrorInfo, bool> logOK = null)
            : base(logOK)
        {
            if (progress == null)
                mProgress = new Progress();
            else
                mProgress = progress;

            //We can't start the spin lock until the progress object has been set.
            mThreadLog.Start();
        } 
        #endregion

        #region ResourcesAcquire()
        /// <summary>
        /// This method sets the resources needed for the writer.
        /// </summary>
        protected override void ResourcesAcquire()
        {
            mReset = new ManualResetEventSlim(false);
            mLogQueue = new ConcurrentQueue<ErrorInfo>();
            mThreadLog = new Thread(SpinWrite);
        }
        #endregion

        #region WriteInternal(ErrorInfo e)
        /// <summary>
        /// This method writes out the data to the console.
        /// </summary>
        /// <param name="e">The logging information.</param>
        protected override void WriteInternal(ErrorInfo e)
        {
            switch (e.Type)
            {
                case LoggingLevel.Error:
                case LoggingLevel.Fatal:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LoggingLevel.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            System.Console.WriteLine(e.Message);
            System.Console.ResetColor();
        } 
        #endregion

        #region ProgressStart()
        /// <summary>
        /// This method allows the delay timer progress bar to appear in the console.
        /// </summary>
        public void ProgressStart()
        {
            if (mDisposed)
                throw new ObjectDisposedException("ProgressStart");

            mProgress.Start();
        } 
        #endregion
        #region ProgressStop()
        /// <summary>
        /// This method stops the progress bar display.
        /// </summary>
        public void ProgressStop()
        {
            if (mDisposed)
                throw new ObjectDisposedException("ProgressStop");

            mProgress.Stop();
        } 
        #endregion

        #region Write(object state, I item)
        /// <summary>
        /// This method writes the mail from the handler.
        /// </summary>
        /// <param name="handler">The mail handler.</param>
        public override void Write(object state, ErrorInfo item)
        {
            if (mDisposed)
                throw new ObjectDisposedException("Write");

            if (mLogOK(item))
            {
                mLogQueue.Enqueue(item);
                mProgress.Reset();
                mReset.Set();
            }
        }
        #endregion
        #region SpinWrite(object state)
        /// <summary>
        /// This method is used to manage logging using a single thread.
        /// </summary>
        /// <param name="state">The logged state.</param>
        protected override void SpinWrite(object state)
        {
            int wait = mProgress.TickWaitInMs;

            while (!mDisposed)
            {
                int count = WriteBuffer();
                if (count == 0)
                    mProgress.ConsoleAction();

                mReset.Reset();
                mReset.Wait(wait);
            }
        }
        #endregion
    }
}
