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
