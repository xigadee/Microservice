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

namespace Xigadee
{
    /// <summary>
    /// This interface is used for TaskTracker queueing.
    /// </summary>
    public interface IQueueTracker
    {
        /// <summary>
        /// Gets the queue level.
        /// </summary>
        int Level { get; }
        /// <summary>
        /// Gets or sets the queue level.
        /// </summary>
        void Configure(int level);
        /// <summary>
        /// Returns true if the queue is empty.
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Enqueues the TaskTracker.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        void Enqueue(TaskTracker item);
        /// <summary>
        /// Tries to dequeue the first item in the queue.
        /// </summary>
        /// <param name="item">As out parameter with the item dequeued.</param>
        /// <returns>Returns true if an item is dequeued.</returns>
        bool TryDequeue(out TaskTracker item);
        /// <summary>
        /// Returns the statistics on the queue.
        /// </summary>
        QueueTrackerStatistics Statistics {get;}
        /// <summary>
        /// Returns the number of items in the queue.
        /// </summary>
        int Count { get; }
    }
}