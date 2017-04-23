using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class OutgoingRequestTransmitException: Exception
    {
        public OutgoingRequestTransmitException(string message) : base(message)
        {
        }
    }
}
