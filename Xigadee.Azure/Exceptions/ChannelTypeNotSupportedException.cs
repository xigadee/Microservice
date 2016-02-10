#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public class ChannelTypeNotSupportedException:Exception
    {
        public ChannelTypeNotSupportedException(string channelType, ServiceMessage message = null)
        {

        }
    }
}
