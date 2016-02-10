#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        private readonly ServiceMessageHeader mHeader;

        public ContractAttribute(string type, string messageType, string messageAction)
        {
            mHeader = new ServiceMessageHeader(type, messageType, messageAction);
        }

        public string ChannelId { get { return mHeader.ChannelId; } }
        public string MessageType { get { return mHeader.MessageType; } }
        public string ActionType { get { return mHeader.ActionType; } }

        public ServiceMessageHeader Header { get { return mHeader; } }
    }
}
