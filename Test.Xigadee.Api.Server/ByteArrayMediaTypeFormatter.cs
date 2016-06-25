using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Test.Xigadee.Api.Server
{
    public class ByteArrayMediaTypeFormatter: MediaTypeFormatter
    {
        public ByteArrayMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpeg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/gif"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew<object>(() => content.ReadAsByteArrayAsync().Result);
        }

        public override bool CanReadType(Type type)
        {
            return typeof(byte[]) == type;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }
    }
}