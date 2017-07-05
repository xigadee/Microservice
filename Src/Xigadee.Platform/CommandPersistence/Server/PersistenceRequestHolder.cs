#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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

        public string Debug { get { return $"{Prq?.Id.ToString("N")}={Prq?.Message?.ToServiceMessageHeader().Key} Retries={mRetry} Extent={Extent.ToFriendlyString()}"; } }

        public TimeSpan? Extent
        {
            get { return ConversionHelper.DeltaAsTimeSpan(Start, Environment.TickCount); }
        }

        public int Retries { get { return mRetry; } }
    }
}
