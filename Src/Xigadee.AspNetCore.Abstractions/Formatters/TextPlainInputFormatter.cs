using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Xigadee
{
    /// <summary>
    /// Formatter that allows content of type text/plain or no content type to be parsed to string data.
    /// Allows for a single input parameter in the form of : public string RawString([FromBody] string data)
    /// </summary>
    public class TextPlainInputFormatter : InputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPlainInputFormatter"/> class.
        /// </summary>
        public TextPlainInputFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaTypeNames.Text.Plain));
        }

        /// <summary>
        /// Allow text/plain and no content type to be processed
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if it can read.</returns>
        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType) || contentType.Contains(MediaTypeNames.Text.Plain))
                return true;

            return false;
        }

        /// <summary>
        /// Processes text plain requests.
        /// </summary>
        /// <param name="context">The format context.</param>
        /// <returns>Returns the formatter.</returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var request = context.HttpContext.Request;
            var contentType = context.HttpContext.Request.ContentType;

            if (string.IsNullOrEmpty(contentType) || contentType.Contains(MediaTypeNames.Text.Plain))
            {
                using (var reader = new StreamReader(request.Body))
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
            }

            return await InputFormatterResult.FailureAsync();
        }
    }
}
