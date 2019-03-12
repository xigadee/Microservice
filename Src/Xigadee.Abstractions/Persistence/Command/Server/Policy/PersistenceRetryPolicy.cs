using System;

namespace Xigadee
{
    /// <summary>
    /// This policy decides the persistence retry behaviour.
    /// </summary>
    public class PersistenceRetryPolicy:PolicyBase
    {
        private readonly Func<TransmissionPayload, int> mMaximumRetries;
        private readonly Func<TransmissionPayload, int, TimeSpan> mDelayBetweenRetries;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="maximumRetries">The maximum retries policy.</param>
        /// <param name="delayBetweenRetries">The delay function.</param>
        public PersistenceRetryPolicy(Func<TransmissionPayload, int> maximumRetries = null, Func<TransmissionPayload, int, TimeSpan> delayBetweenRetries = null)
        {
            mMaximumRetries = maximumRetries ?? DefaultMaximumRetries;
            mDelayBetweenRetries = delayBetweenRetries ?? DefaultDelayBetweenRetries;
        }

        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="t">The name of the resource profile.</param>
        public static implicit operator PersistenceRetryPolicy(ValueTuple<Func<TransmissionPayload, int>> t)
        {
            return new PersistenceRetryPolicy(t.Item1);
        }
        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="t">The name of the resource profile.</param>
        public static implicit operator PersistenceRetryPolicy(ValueTuple<Func<TransmissionPayload, int>, Func<TransmissionPayload, int, TimeSpan>> t)
        {
            return new PersistenceRetryPolicy(t.Item1, t.Item2);
        }


        /// <summary>
        /// This method returns the maximum number of retries permitted.
        /// </summary>
        /// <param name="transmissionPayload">The payload to inspect.</param>
        /// <returns>Returns the retry settings.</returns>
        public virtual int GetMaximumRetries(TransmissionPayload transmissionPayload)
        {
            return mMaximumRetries(transmissionPayload);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transmissionPayload"></param>
        /// <param name="retryNumber"></param>
        /// <returns></returns>
        public virtual TimeSpan GetDelayBetweenRetries(TransmissionPayload transmissionPayload, int retryNumber = 0)
        {
            return mDelayBetweenRetries(transmissionPayload, retryNumber);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transmissionPayload"></param>
        /// <returns></returns>
        protected virtual int DefaultMaximumRetries(TransmissionPayload transmissionPayload)
        {
            // Channel 0 async processing should keep retrying
            if (transmissionPayload !=null && transmissionPayload.Message != null && transmissionPayload.Message.ChannelPriority == 0)
                return 500;

            return 5;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transmissionPayload"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        protected virtual TimeSpan DefaultDelayBetweenRetries(TransmissionPayload transmissionPayload, int i)
        {
            if (transmissionPayload != null && transmissionPayload.Message != null && transmissionPayload.Message.ChannelPriority == 0)
                return TimeSpan.FromMilliseconds(i * 10);

            return TimeSpan.FromSeconds(i);
        }
    }
}
