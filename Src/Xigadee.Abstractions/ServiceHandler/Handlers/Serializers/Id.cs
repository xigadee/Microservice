using System;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// The serialization id.
    /// </summary>
    /// <seealso cref="Xigadee.ServiceHandlerIdBase" />
    [DebuggerDisplay("{Raw}")]
    public class SerializationHandlerId : ServiceHandlerIdBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationHandlerId"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public SerializationHandlerId(string id) : base(id) { }

        /// <summary>
        /// This is the object type.
        /// </summary>
        public string ObjectType { get; protected set; }

        /// <summary>
        /// Processes the incoming identifier and strips off any additional parameters.
        /// </summary>
        /// <param name="id">The incoming identifier.</param>
        /// <returns>
        /// The processed identifier.
        /// </returns>
        public override string ProcessIdentifier(string id)
        {
            var items = id.Split(';');

            var value = items[0].Trim().ToLowerInvariant();

            for(int i = 1; i < items.Length; i++)
            {
                var item = items[i].Trim();
                if (item.StartsWith("type", StringComparison.InvariantCultureIgnoreCase))
                {
                    var parts = item.Split('=');
                    ObjectType = parts.Length == 2 ? parts[1].Trim('\"') : string.Empty;
                    break;
                }
            }

            return value;
        }

        /// <summary>
        /// Implicitly converts a string in to an id.
        /// </summary>
        /// <param name="id">The name of the resource profile.</param>
        public static implicit operator SerializationHandlerId(string id)
        {
            if (id == null)
                return null;

            return new SerializationHandlerId(id);
        }

        /// <summary>
        /// Implicitly converts a handler id to a string.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(SerializationHandlerId handlerId)
        {
            if (handlerId == null)
                return null;

            return handlerId.Id;
        }
    }
}
