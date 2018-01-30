using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This collection contains the handlers for a particular group.
    /// </summary>
    [DebuggerDisplay("{Count}")]
    public class ServiceHandlerCollection<I>: IEnumerable<I> //:StatisticsBase<
        where I: class, IServiceHandler
    {
        Dictionary<string,I> mHandlers = new Dictionary<string, I>();
        Action<I> mOnAdd;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHandlerCollection{I}"/> class.
        /// </summary>
        /// <param name="onAdd">The optional on-add action that can be used to set 
        /// additional handler properties when adding an item to the collection.</param>
        public ServiceHandlerCollection(Action<I> onAdd = null)
        {
            mOnAdd = onAdd;
        } 
        #endregion

        #region Contains ...
        /// <summary>
        /// Determines whether the handler id is contained in the collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if the collection [contains] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public bool Contains(string id)
        {
            return mHandlers.ContainsKey(id);
        }

        /// <summary>
        /// Determines whether the handler is contained in the collection.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns><c>true</c> if the collection [contains] [the specified handler]; otherwise, <c>false</c>.</returns>
        public bool Contains(I handler)
        {
            return Contains(handler.Id);
        } 
        #endregion

        #region Add ...
        /// <summary>
        /// Adds the specified handler through the use of a creation function.
        /// </summary>
        /// <param name="creator">The handler creator function.</param>
        /// <returns>Returns the new handler if successful.</returns>
        public I Add(Func<I> creator)
        {
            return Add(creator());
        }

        /// <summary>
        /// Adds the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns the incoming handler if successful.</returns>
        /// <exception cref="ArgumentException">handler.Id - The handler.Id parameter cannot be null or an empty string</exception>
        /// <exception cref="ArgumentException">handler.Id - The handler.Id already exists in the collection</exception>
        public I Add(I handler)
        {
            if (string.IsNullOrWhiteSpace(handler.Id))
                throw new ArgumentException("handler.Id", "The handler.Id parameter cannot be null or an empty string");

            mOnAdd?.Invoke(handler);
            mHandlers.Add(handler.Id, handler);
            return handler;
        } 
        #endregion

        #region TryGet(string id, out I handler)
        /// <summary>
        /// Tries to get the specific handler.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>True if the handler exists.</returns>
        public bool TryGet(string id, out I handler)
        {
            return mHandlers.TryGetValue(id, out handler);
        } 
        #endregion
        #region TryRemove
        /// <summary>
        /// Tries to remove a handler from the collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if the handler has been removed successfully.</returns>
        public bool TryRemove(string id)
        {
            I handler;
            return TryRemove(id, out handler);
        } 
        /// <summary>
        /// Tries to remove a handler from the collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>Returns true if the handler has been removed successfully.</returns>
        public bool TryRemove(string id, out I handler)
        {
            handler = null;

            if (!mHandlers.TryGetValue(id, out handler))
                return false;

            return mHandlers.Remove(id);
        }
        #endregion
        #region Clear()
        /// <summary>
        /// Clears the collection..
        /// </summary>
        public void Clear()
        {
            mHandlers.Clear();
        }
        #endregion

        #region GetEnumerator()
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<I> GetEnumerator()
        {
            return mHandlers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 
        #endregion

        #region Count
        /// <summary>
        /// Gets the collection count.
        /// </summary>
        public int Count => mHandlers?.Count ?? 0; 
        #endregion
    }
}
