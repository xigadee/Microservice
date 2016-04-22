#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    [DebuggerDisplay("{Debug}")]
    public class PersistenceRequestHolder<KT, ET>: IPersistenceRequestHolder
    {
        private int mRetry;

        public PersistenceRequestHolder(Guid profileId, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            this.ProfileId = profileId;
            this.prq = prq;
            this.prs = prs;

            Start = Environment.TickCount;

            result = null;
            rq = null;
            rs = null;
        }

        public string Debug { get { return $"{prq?.Id.ToString("N")}={prq?.Message?.ToServiceMessageHeader().ToKey()} Retry={mRetry} Extent={ConversionHelper.DeltaAsFriendlyTime(Start, Environment.TickCount)}";} }


        public PersistenceRepositoryHolder<KT, ET> rq;

        public PersistenceRepositoryHolder<KT, ET> rs;

        public TransmissionPayload prq;

        public List<TransmissionPayload> prs;

        public int Start { get; private set; }

        public Guid ProfileId { get; private set; }

        public ResourceRequestResult? result;

        public void Retry(int retryStart)
        {
            Interlocked.Increment(ref mRetry);
        }

    }
}
