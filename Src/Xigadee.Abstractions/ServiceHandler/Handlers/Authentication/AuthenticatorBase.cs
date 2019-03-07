using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public abstract class AuthenticatorBase : ServiceHandlerBase, IServiceHandlerAuthentication
    {
        public MicroserviceId OriginatorId { get; set; }

        public IDataCollection Collector { get; set; }

        public void Sign(TransmissionPayload payload)
        {
            throw new NotImplementedException();
        }

        public void Verify(TransmissionPayload payload)
        {
            throw new NotImplementedException();
        }
    }
}
