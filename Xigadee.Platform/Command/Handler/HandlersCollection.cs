#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This collection holds the handlers.
    /// </summary>
    public class HandlersCollection: ISupportedMessageTypes
    {
        private Func<List<MessageFilterWrapper>> mSupportedMessageTypes;

        public event EventHandler<SupportedMessagesChange> OnCommandChange;

        public HandlersCollection(Func<List<MessageFilterWrapper>> supportedMessageTypes)
        {
            mSupportedMessageTypes = supportedMessageTypes;
        }

        public void NotifyChange(List<MessageFilterWrapper> messages)
        {
            try
            {
                OnCommandChange?.Invoke(this, new SupportedMessagesChange() { Messages = mSupportedMessageTypes() });
            }
            catch (Exception ex)
            {
                //Logger?.
            }
        }

        public List<MessageFilterWrapper> SupportedMessages
        {
            get
            {
                return mSupportedMessageTypes();
            }
        }
    }
}
