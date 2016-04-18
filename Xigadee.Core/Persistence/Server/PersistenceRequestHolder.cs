#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the incoming persistence request while it is being processed.
    /// </summary>
    /// <typeparam name="KT">The key type.</typeparam>
    /// <typeparam name="ET">The entity type.</typeparam>
    public class PersistenceRequestHolder<KT, ET>: IPersistenceRequestHolder
    {
        private int mRetry;

        public PersistenceRequestHolder(Guid profileId, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            this.profileId = profileId;
            this.prq = prq;
            this.prs = prs;

            start = Environment.TickCount;

            result = null;
            rq = null;
            rs = null;
        }

        public PersistenceRepositoryHolder<KT, ET> rq;

        public PersistenceRepositoryHolder<KT, ET> rs;

        public TransmissionPayload prq;

        public List<TransmissionPayload> prs;

        public int start { get; private set; }

        public Guid profileId { get; private set; }

        public ResourceRequestResult? result;

        public void Retry(int retryStart)
        {
            Interlocked.Increment(ref mRetry);
        }

        public string Profile
        {
            get
            {
                return "";
            }
        }
    }
}
