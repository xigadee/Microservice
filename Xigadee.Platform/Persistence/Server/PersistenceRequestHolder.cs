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
            this.Prq = prq;
            this.Prs = prs;

            Start = Environment.TickCount;

            result = null;
            Rq = null;
            Rs = null;
        }

        public TransmissionPayload Prq { get; }

        public List<TransmissionPayload> Prs { get; }


        public PersistenceRepositoryHolder<KT, ET> Rq { get; set; }

        public PersistenceRepositoryHolder<KT, ET> Rs { get; set; }



        public int Start { get; }

        public Guid ProfileId { get; }

        public ResourceRequestResult? result { get; set; }

        public void Retry(int retryStart)
        {
            Interlocked.Increment(ref mRetry);
        }

        public string Debug { get { return $"{Prq?.Id.ToString("N")}={Prq?.Message?.ToServiceMessageHeader().ToKey()} Retries={mRetry} Extent={Extent.ToFriendlyString()}"; } }

        public TimeSpan? Extent
        {
            get { return ConversionHelper.DeltaAsTimeSpan(Start, Environment.TickCount); }
        }

        public int Retries { get { return mRetry; } }
    }
}
