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

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceRequestHolder{KT, ET}"/> class.
        /// </summary>
        /// <param name="profileId">The profile identifier.</param>
        /// <param name="prq">The request.</param>
        /// <param name="prs">The response collection.</param>
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
        /// <summary>
        /// The request payload.
        /// </summary>
        public TransmissionPayload Prq { get; }
        /// <summary>
        /// The response payload collection.
        /// </summary>
        public List<TransmissionPayload> Prs { get; }

        /// <summary>
        /// The request.
        /// </summary>
        public PersistenceRepositoryHolder<KT, ET> Rq { get; set; }
        /// <summary>
        /// The response.
        /// </summary>
        public PersistenceRepositoryHolder<KT, ET> Rs { get; set; }


        /// <summary>
        /// The tick count start point.
        /// </summary>
        public int Start { get; }
        /// <summary>
        /// The profile identifier.
        /// </summary>
        public Guid ProfileId { get; }
        /// <summary>
        /// The current request result.
        /// </summary>
        public ResourceRequestResult? result { get; set; }
        /// <summary>
        /// Increments the retry counter.
        /// </summary>
        /// <param name="retryStart">The retry start.</param>
        public void Retry(int retryStart)
        {
            Interlocked.Increment(ref mRetry);
        }
        /// <summary>
        /// The debug string for the statistics collection.
        /// </summary>
        public string Debug { get { return $"{Prq?.Id.ToString("N")}={Prq?.Message?.ToServiceMessageHeader().Key} Retries={mRetry} Extent={Extent.ToFriendlyString()}"; } }
        /// <summary>
        /// The current extent from the start of the request.
        /// </summary>
        public TimeSpan? Extent
        {
            get { return ConversionHelper.DeltaAsTimeSpan(Start, Environment.TickCount); }
        }
        /// <summary>
        /// The current retry count.
        /// </summary>
        public int Retries { get { return mRetry; } }
    }
}
