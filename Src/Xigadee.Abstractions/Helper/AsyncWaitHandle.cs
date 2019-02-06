using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// Extension methods to add async behaviour to the WaitHandle classes
    /// </summary>
    public static class AsyncWaitHandle
    {
        /// <summary>
        /// Extension method to make WaitOne awaitable
        /// </summary>
        /// <param name="waitHandle">WaitHandle</param>
        /// <param name="millisecondsTimeout">Timeout in milliseconds to wait</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static async Task<bool> WaitOneAsync(this WaitHandle waitHandle, int millisecondsTimeout
            , CancellationToken cancellationToken)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default(CancellationTokenRegistration);
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    waitHandle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);

                tokenRegistration = cancellationToken.Register(state => ((TaskCompletionSource<bool>)state).TrySetCanceled(), tcs);
                return await tcs.Task;
            }
            finally
            {
                registeredHandle?.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        /// <summary>
        /// Extension method to make WaitOne awaitable
        /// </summary>
        /// <param name="waitHandle">WaitHandle</param>
        /// <param name="timeout">Timeout for waiting</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return waitHandle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Extension method to make WaitOne awaitable
        /// </summary>
        /// <param name="waitHandle">WaitHandle</param>
        /// <param name="timeout">Timeout for waiting</param>
        /// <returns></returns>
        public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, TimeSpan timeout)
        {
            return waitHandle.WaitOneAsync((int)timeout.TotalMilliseconds, new CancellationTokenSource().Token);
        }

        /// <summary>
        /// Extension method to make WaitOne awaitable
        /// </summary>
        /// <param name="waitHandle">WaitHandle</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static Task<bool> WaitOneAsync(this WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            return waitHandle.WaitOneAsync(Timeout.Infinite, cancellationToken);
        }
    }
}
