using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class TransportVerificationTests
    {
        [TestClass]
        public class CommunicationBridgeTests
        {
            /// <summary>
            /// This method test the connectivity between multiple Microservices
            /// 
            /// [Sender] (t:id1) /o-|crequest|-/i [Receiver]
            /// [Sender] \i-|cresponse|-\o [Receiver]
            /// 
            /// </summary>
            //[Ignore]
            [TestMethod]
            public void TestMethod1()
            {
                try
                {
                    var fabric = new ManualCommunicationFabric();
                    var bridgeOut = fabric[CommunicationFabricMode.Queue];
                    var bridgein = fabric[CommunicationFabricMode.Broadcast];

                    PersistenceClient<Guid, SecureMe> init;
                    DebugMemoryDataCollector memp1, memp2;

                    var p1 = new MicroservicePipeline("Sender")
                        .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                        .AddAuthenticationHandlerJwtToken("id1", JwtHashAlgorithm.HS256, Encoding.UTF8.GetBytes("My big secret"))
                        .AddDebugMemoryDataCollector(out memp1)
                        .AddChannelIncoming("cresponse", boundaryLoggingEnabled: true)
                            .AttachListener(bridgein.GetListener())
                            .Revert()
                        .AddChannelOutgoing("crequest", boundaryLoggingEnabled: true)
                            .AttachSender(bridgeOut.GetSender())
                            .AttachTransportPayloadSignature("id1")
                            .AttachPersistenceClient("cresponse", out init)
                            .Revert()
                            ;

                    var p2 = new MicroservicePipeline("Receiver")
                        .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                        .AddAuthenticationHandlerJwtToken("id1", JwtHashAlgorithm.HS256, Encoding.UTF8.GetBytes("My big secret"))
                        .AddDebugMemoryDataCollector(out memp2)
                        .AddChannelIncoming("crequest", boundaryLoggingEnabled: true)
                            .AttachListener(bridgeOut.GetListener())
                            .AttachTransportPayloadVerification("id1")
                            .AttachCommand(new PersistenceManagerHandlerMemory<Guid, SecureMe>((e) => e.Id, (s) => new Guid(s)))
                            .Revert()
                        .AddChannelOutgoing("cresponse", boundaryLoggingEnabled: true)
                            .AttachSender(bridgein.GetSender())
                            .Revert()
                            ;

                    p1.Start();      
                    p2.Start();

                    int check1 = p1.ToMicroservice().Commands.Count();
                    int check2 = p2.ToMicroservice().Commands.Count();

                    var entity = new SecureMe() { Message = "Momma" };
                    var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(30) }).Result;
                    var rs2 = init.Read(entity.Id).Result;

                    Assert.IsTrue(rs2.IsSuccess);
                    Assert.IsTrue(rs2.Entity.Message == "Momma");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public class SecureMe
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Message { get; set; }
        }
    }
}
