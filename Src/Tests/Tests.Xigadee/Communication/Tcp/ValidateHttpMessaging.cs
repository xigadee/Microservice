using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Test.Xigadee.Communication.Tcp
{
    [TestClass]
    //[Ignore]
    public class ValidateHttpMessaging
    {
        [TestMethod]
        public void HttpRequests()
        {
            try
            {
                var testHTTP_RQ1 =
                    new MessageLoad<HttpProtocolRequestMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http1_rq.txt, Tests.Xigadee");

                var testHTTP_RQ2 =
                    new MessageLoad<HttpProtocolRequestMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http2_rq.txt, Tests.Xigadee");

                var testHTTP_RQ3 =
                    new MessageLoad<HttpProtocolRequestMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http3_rq.txt, Tests.Xigadee");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [TestMethod]
        public void HttpResponses()
        {
            try
            {
                var testHTTP_RS1 =
                    new MessageLoad<HttpProtocolResponseMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http1_rs.txt, Tests.Xigadee");

                var testHTTP_RS2 =
                    new MessageLoad<HttpProtocolResponseMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http2_rs.txt, Tests.Xigadee");

                var testHTTP_RS3 =
                    new MessageLoad<HttpProtocolResponseMessage>("Tests.Xigadee.Communication.Tcp.HttpSamples.http3_rs.txt, Tests.Xigadee");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [TestMethod]
        public void HttpRequestContinuity()
        {
            try
            {
                using (Stream sResource = MessageLoad<HttpProtocolRequestMessage>.ResourceLoadStream(
                    "Tests.Xigadee.Communication.Tcp.HttpSamples.http1_rq_cont.txt, Tests.Xigadee"))
                {
                    var tempMessage = new HttpProtocolRequestMessage();
                    var tempMessage2 = new HttpProtocolRequestMessage();

                    tempMessage.Load();
                    tempMessage2.Load();

                    var result = tempMessage.WriteFromStream(sResource);

                    if (result.overread.HasValue)
                    {
                        var result2 = tempMessage2.WriteFromStream(sResource, result.overread);
                    }

                    Assert.IsFalse(tempMessage.CanRead);
                    Assert.IsFalse(tempMessage2.CanRead);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// This class is used to load a resource or a stream in to a message.
    /// </summary>
    /// <typeparam name="M">The message type.</typeparam>
    public class MessageLoad<M>
        where M : Message, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. This loads the resource in to the message.
        /// </summary>
        /// <param name="resource">The resource.</param>
        public MessageLoad(string resource)
        {
            using (Stream sResource = ResourceLoadStream(resource))
            {
                Initialize(sResource);
            }
        }
        /// <summary>
        /// This is the default constructor. This loads the resource in to the message.
        /// </summary>
        /// <param name="resource">The resource stream.</param>
        public MessageLoad(Stream resource)
        {
            Initialize(resource);
        }
        #endregion 
        #region Initialize(Stream sData)
        /// <summary>
        /// This method initializes the message.
        /// </summary>
        /// <param name="sData">The stream data.</param>
        protected virtual void Initialize(Stream sData)
        {
            M tempMessage = new M();

            tempMessage.Load();

            var result = tempMessage.WriteFromStream(sData);

            //while (sData.CanRead)
            //{
            //    int value = sData.Read(blob, 0, 1000);
            //    if (value == 0)
            //        break;

            //    tempMessage.Write(blob, 0, value);
            //}

            Data = tempMessage;
        }
        #endregion 

        #region Data
        /// <summary>
        /// This is the message.
        /// </summary>
        public M Data { get; set; }
        #endregion

        #region ResourceLoadStream(string resourceName)
        /// <summary>
        /// This method reads a binary definition from an assembly based on the resource name.
        /// Note: this method will attempt to load the assembly if it is not loaded.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>Returns an unmanaged stream containing the data.</returns>
        public static Stream ResourceLoadStream(string resourceName)
        {
            string[] items = resourceName.Split(new char[] { ',' }).Select(i => i.Trim()).ToArray();

            Assembly ass = Assembly.GetExecutingAssembly();

            if (items.Length > 1)
            {
                var asses = AppDomain.CurrentDomain.GetAssemblies();
                ass = asses.SingleOrDefault(a => a.FullName.ToLowerInvariant().StartsWith(items[1].ToLowerInvariant()));

                if (ass == null)
                {
                    ass = AppDomain.CurrentDomain.Load(items[1]);

                    if (ass == null)
                        throw new ArgumentOutOfRangeException(
                            string.Format("The assembly cannot be resolved: {0}", items[1]));
                }
            }

            return ass.GetManifestResourceStream(items[0]);
        }
        #endregion 
    }
}
