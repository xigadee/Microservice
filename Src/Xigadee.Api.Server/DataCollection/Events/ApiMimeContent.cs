#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
#endregion

namespace Xigadee
{
    /// <summary>
    /// This class holds the incoming and outgoing content.
    /// </summary>
    public class ApiMimeContent
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="content">The http content.</param>
        public ApiMimeContent(HttpContent content)
        {
            if (content == null || content.Headers.ContentLength == 0)
                return;

            IEnumerable<string> contentTypes;
            if (content.Headers.TryGetValues("Content-Type", out contentTypes))
                ContentType = contentTypes.FirstOrDefault();

            try
            {
                Body = content.ReadAsByteArrayAsync().Result;
            }
            catch (Exception)
            {
                // Do not cause the application to throw an exception due to logging failure
            }
        }
        /// <summary>
        /// This is the payload content type.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// This is the binary payload body.
        /// </summary>
        public byte[] Body { get; }
    }
}
