using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This static class is provides helper methods for the HttpContent.
    /// </summary>
    public static class HttpContentHelper
    {
        /// <summary>
        /// This shortcut checks whether the Http Content has data.
        /// </summary>
        /// <param name="httpRs"></param>
        /// <returns></returns>
        public static bool HasContent(this HttpResponseMessage httpRs) => (httpRs.Content != null && (httpRs.Content.Headers.ContentLength ?? 0) > 0);

        /// <summary>
        /// Converts the object in to a default UTF8 encoded Json HTTPContent body.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Returns a Http content object.</returns>
        public static HttpContent ConvertObject(object model) => ConvertJsonUTF8(JsonConvert.SerializeObject(model));
        /// <summary>
        /// Converts the string in to a UTF8 encoded Json HTTPContent body.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>Returns the HttpContent object.</returns>
        public static HttpContent ConvertJsonUTF8(string json) => ConvertUTF8(json, "application/json");
        /// <summary>
        /// Converts the texts in to a http UTF8 encoded content body.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="mimeType">The optional mime type. The default is text/plain.</param>
        /// <returns>Returns the HTTP content object.</returns>
        public static HttpContent ConvertUTF8(string text, string mimeType = "text/plain")
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            var content = new ByteArrayContent(data);
            content.Headers.Add("Content-Type", $"{mimeType}; charset=utf-8");
            return content;
        }

        /// <summary>
        /// Converts the object in to a HttpContent using either the serializer function or the default UTF8 encoded Json HTTPContent body.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <param name="requestmodelSerializer">The optional model serializer.</param>
        /// <returns>Returns the HttpContent object.</returns>
        public static HttpContent Convert(object model, Func<object, HttpContent> requestmodelSerializer = null)
        {
            HttpContent content = null;

            if (model != null)
            {
                if (requestmodelSerializer == null)
                    content = ConvertObject(model);
                else
                    content = requestmodelSerializer(model);
            }

            return content;
        }

        /// <summary>
        /// This method converts the incoming http content to the object.
        /// </summary>
        /// <typeparam name="O">The object type.</typeparam>
        /// <param name="content">The Http content.</param>
        /// <returns>The deserialized object.</returns>
        public static async Task<O> FromJsonUTF8<O>(this HttpContent content)
            where O : class
        {
            byte[] httpRsContent = await content.ReadAsByteArrayAsync();

            return JsonConvert.DeserializeObject<O>(Encoding.UTF8.GetString(httpRsContent));
        }

        
    }
}
