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

using System;

namespace Xigadee
{
    /// <summary>
    /// This policy decides the persistence retry behavior.
    /// </summary>
    public class PersistenceRetryPolicy:PolicyBase
    {
        private readonly Func<TransmissionPayload, int> mMaximumRetries;
        private readonly Func<TransmissionPayload, int, TimeSpan> mDelayBetweenRetries;

        public PersistenceRetryPolicy(Func<TransmissionPayload, int> maximumRetries = null, Func<TransmissionPayload, int, TimeSpan> delayBetweenRetries = null)
        {
            mMaximumRetries = maximumRetries ?? DefaultMaximumRetries;
            mDelayBetweenRetries = delayBetweenRetries ?? DefaultDelayBetweenRetries;
        }

        public virtual int GetMaximumRetries(TransmissionPayload transmissionPayload)
        {
            return mMaximumRetries(transmissionPayload);
        }

        public virtual TimeSpan GetDelayBetweenRetries(TransmissionPayload transmissionPayload, int retryNumber = 0)
        {
            return mDelayBetweenRetries(transmissionPayload, retryNumber);
        }

        protected virtual int DefaultMaximumRetries(TransmissionPayload transmissionPayload)
        {
            // Channel 0 async processing should keep retrying
            if (transmissionPayload !=null && transmissionPayload.Message != null && transmissionPayload.Message.ChannelPriority == 0)
                return 500;

            return 5;
        }

        protected virtual TimeSpan DefaultDelayBetweenRetries(TransmissionPayload transmissionPayload, int i)
        {
            if (transmissionPayload != null && transmissionPayload.Message != null && transmissionPayload.Message.ChannelPriority == 0)
                return TimeSpan.FromMilliseconds(i * 10);

            return TimeSpan.FromSeconds(i);
        }
    }
}
