using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class manages the Uri mapping for a particular key.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    public class TransportUriMapper<K>
    {
        /// <summary>
        /// This method is used to 
        /// </summary>
        protected IKeyMapper<K> mKeyMapper;

        Dictionary<HttpMethod, string> mUriTemplates;

        public TransportUriMapper(IKeyMapper<K> keyMapper = null)
        {
            mUriTemplates = new Dictionary<HttpMethod, string>();

            UseHttps = true;

            if (keyMapper != null)
                mKeyMapper = keyMapper;
            else
                mKeyMapper = (KeyMapper<K>)KeyMapper.Resolve<K>();
        }

        /// <summary>
        /// This property determines whether to use https to call the Api.
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// This is the scheme, i.e. http or https
        /// </summary>
        public virtual string Scheme { get { return UseHttps ? "https" : "http"; } }

        /// <summary>
        /// This is the 
        /// </summary>
        public virtual Uri Server { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Path { get; set; }

        /// <summary>
        /// This method returns a UriBuilder for the request.
        /// </summary>
        /// <param name="method">The current method.</param>
        /// <returns>Returns the builder.</returns>
        protected virtual UriBuilder UriParts(HttpMethod method)
        {
            var builder = new UriBuilder(Server);

            builder.Scheme = Scheme;
            builder.Path = string.Format("{0}{1}", builder.Path, Path);

            return builder;
        }

        public virtual KeyValuePair<HttpMethod,Uri> MakeUri(HttpMethod method)
        {
            var builder = UriParts(method);
            return new KeyValuePair<HttpMethod, Uri>(method, builder.Uri);
        }

        public virtual KeyValuePair<HttpMethod, Uri> MakeUri(HttpMethod method, K key)
        {
            var builder = UriParts(method);
            
            builder.Path = string.Format("{0}/{1}", builder.Path, Uri.EscapeDataString(mKeyMapper.ToString(key)));
            return new KeyValuePair<HttpMethod, Uri>(method, builder.Uri);
        }

        public virtual KeyValuePair<HttpMethod, Uri> MakeUri(HttpMethod method, string refKey, string refValue)
        {
            var builder = UriParts(method);
            builder.Query = string.Format("reftype={0}&refvalue={1}", Uri.EscapeDataString(refKey), Uri.EscapeDataString(refValue));
            return new KeyValuePair<HttpMethod, Uri>(method, builder.Uri);

        }
    }
}
