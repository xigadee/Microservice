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

            [TestMethod]
            public void TestMethod1()
            {
                try
                {

                    var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                    var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                    PersistenceMessageInitiator<Guid, SecureMe> init;
                    DebugMemoryDataCollector memp1, memp2;

                    var p1 = new MicroservicePipeline("Sender")
                        .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                        .AddAuthenticationHandlerJwtToken("id1", JwtHashAlgorithm.HS256
                            , Encoding.UTF8.GetBytes("My big secret"))
                        .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp1 = c)
                        .AddChannelOutgoing("crequest", boundaryLoggingEnabled: true)
                            .AttachSender(bridgeOut.GetSender())
                            .AttachTransportPayloadSignature("id1")
                            .Revert()
                        .AddChannelIncoming("cresponse", boundaryLoggingEnabled: true)
                            .AttachListener(bridgein.GetListener())
                            .AttachPersistenceMessageInitiator(out init, "crequest")
                            ;

                    var p2 = new MicroservicePipeline("Receiver")
                        .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                        .AddAuthenticationHandlerJwtToken("id1", JwtHashAlgorithm.HS256
                            , Encoding.UTF8.GetBytes("My big secret"))
                        .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp2 = c)
                        .AddChannelIncoming("crequest", boundaryLoggingEnabled: true)
                            .AttachListener(bridgeOut.GetListener())
                            .AttachTransportPayloadVerification("id1")
                            .AttachCommand(new PersistenceManagerHandlerMemory<Guid, SecureMe>((e) => e.Id, (s) => new Guid(s)))
                            .Revert()
                        .AddChannelOutgoing("cresponse", boundaryLoggingEnabled: true)
                            .AttachSender(bridgein.GetSender())
                            ;

                    p1.Start();
                    p2.Start();

                    int check1 = p1.ToMicroservice().Commands.Count();
                    int check2 = p2.ToMicroservice().Commands.Count();

                    var entity = new SecureMe() { Message = "Momma" };
                    var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
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
