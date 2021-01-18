using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace Xigadee
{
    /// <summary>
    /// Extensions methods for SQL to provide retry logic when transient faults are detected
    /// See: https://github.com/azurevoodoo/AzureSQLTransientHandling/blob/master/src/AzureSQLTransientHandling/PollySqlExtensions.cs. 
    /// Extended Details are here https://hackernoon.com/azure-sql-transient-errors-7625ad6e0a06 
    /// </summary>
    public static class SqlRetryHelper
    {       
        // Transient SQL Errors
        private const int SqlErrorUnknownNetworkError = -1;
        private const int SqlErrorNetworkRelatedError = 5;
        private const int SqlErrorPhysicalConnectionIsNotUsable = 19;
        private const int SqlErrorInstanceDoesNotSupportEncryption = 20;
        private const int SqlErrorConnectedButLoginFailed = 64;
        private const int SqlErrorUnableToEstablishConnection = 233;
        private const int SqlErrorTransportLevelErrorReceivingResult = 10053;
        private const int SqlErrorTransportLevelErrorWhenSendingRequestToServer = 10054;
        private const int SqlErrorNetworkRelatedErrorDuringConnect = 10060;
        private const int SqlErrorDatabaseLimitReached = 10928;
        private const int SqlErrorResourceLimitReached = 10929;        
        private const int SqlErrorLoginFailure = 18456;        
        private const int SqlErrorServiceErrorEncountered = 40197;
        private const int SqlErrorServiceBusy = 40501;        
        private const int SqlErrorServiceRequestProcessFail = 40540;
        private const int SqlErrorServiceExperiencingAProblem = 40545;
        private const int SqlErrorDatabaseUnavailable = 40613;
        private const int SqlErrorOperationInProgress = 40627;

        private const int SqlRetryCount = 5;

        private static TimeSpan ExponentialBackOff(int attempt) { return TimeSpan.FromSeconds(Math.Pow(2, attempt)); }

        /// <summary>
        /// Idempotent Sql Retry Policy for operations that are idempotent
        /// </summary>
        private static readonly AsyncPolicy IdempotentSqlRetryAsyncPolicy = Policy
            .Handle<TimeoutException>()
            .Or<SqlException>()
            .WaitAndRetryAsync(SqlRetryCount, ExponentialBackOff, LogPollyRetry)
            .WithPolicyKey(nameof(IdempotentSqlRetryAsyncPolicy));

        /// <summary>
        /// Standard Sql Retry Policy for operations that are not idempotent
        /// </summary>
        private static readonly AsyncPolicy SqlRetryAsyncPolicy = Policy
            .Handle<TimeoutException>()
            .Or<SqlException>(AnyRetryableError)
            .WaitAndRetryAsync(SqlRetryCount, ExponentialBackOff, LogPollyRetry)
            .WithPolicyKey(nameof(SqlRetryAsyncPolicy));

        private static ILogger _logger;

        private static bool AnyRetryableError(SqlException exception)
        {
            var sqlErrors = exception.Errors.OfType<SqlError>().ToList();
            if (sqlErrors.Any(RetryableError))
                return true;

            // Log out error numbers to allow us to diagnose if this error should be added to the list 
            _logger?.LogWarning($"{exception.Message} - no retryable SQL Error found {string.Join(",", sqlErrors.Select(e => $"({e.Number}:{e.Message})"))}");
            return false;
        }

        private static bool RetryableError(SqlError error)
        {
            switch (error.Number)
            {
                case SqlErrorOperationInProgress:
                case SqlErrorDatabaseUnavailable:
                case SqlErrorServiceExperiencingAProblem:
                case SqlErrorServiceRequestProcessFail:
                case SqlErrorServiceBusy:
                case SqlErrorServiceErrorEncountered:
                case SqlErrorLoginFailure:
                case SqlErrorResourceLimitReached:
                case SqlErrorDatabaseLimitReached:
                case SqlErrorNetworkRelatedErrorDuringConnect:
                case SqlErrorTransportLevelErrorWhenSendingRequestToServer:
                case SqlErrorTransportLevelErrorReceivingResult:
                case SqlErrorUnableToEstablishConnection:
                case SqlErrorConnectedButLoginFailed:
                case SqlErrorInstanceDoesNotSupportEncryption:
                case SqlErrorPhysicalConnectionIsNotUsable:
                case SqlErrorNetworkRelatedError:
                case SqlErrorUnknownNetworkError:
                    return true;

                default:
                    return false;
            }
        }

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Open a SQL connection using an idempotent retry policy
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static async Task<SqlConnection> OpenWithRetryAsync(this SqlConnection conn)
        {
            await IdempotentSqlRetryAsyncPolicy.ExecuteAsync(ctx => conn.OpenAsync(), new Context(nameof(conn.OpenAsync)));
            return conn;
        }

        /// <summary>
        /// Execute a reader asynchronously using a retry policy
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="isIdempotent"></param>
        /// <returns></returns>
        public static Task<SqlDataReader> ExecuteReaderWithRetryAsync(this SqlCommand cmd, bool isIdempotent = false)
        {
            var policy = isIdempotent ? IdempotentSqlRetryAsyncPolicy : SqlRetryAsyncPolicy;
            return policy.ExecuteAsync(async ctx =>
            {
                await VerifyConnectionIsOpen(cmd.Connection);
                return await cmd.ExecuteReaderAsync();
            }, new Context(cmd.CommandText));
        }

        /// <summary>
        /// Execute non query asynchronously using a retry policy
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="isIdempotent"></param>
        /// <returns></returns>
        public static Task<int> ExecuteNonQueryWithRetryAsync(this SqlCommand cmd, bool isIdempotent = false)
        {
            var policy = isIdempotent ? IdempotentSqlRetryAsyncPolicy : SqlRetryAsyncPolicy;
            return policy.ExecuteAsync(async ctx =>
            {
                await VerifyConnectionIsOpen(cmd.Connection);
                return await cmd.ExecuteNonQueryAsync();
            }, new Context(cmd.CommandText));
        }

        /// <summary>
        /// Verify that the SQL Connection is open and if not open it
        /// </summary>
        /// <param name="conn">Sql Connection</param>
        /// <returns></returns>
        private static async Task VerifyConnectionIsOpen(SqlConnection conn)
        {
            if (conn == null || conn.State == ConnectionState.Open)
                return;
            
            switch (conn.State)
            {
                case ConnectionState.Closed:
                {
                    _logger?.LogWarning("Connection was closed. Attempting to re-open");
                    await conn.OpenWithRetryAsync();
                    break;
                }
                case ConnectionState.Broken:
                {
                    _logger?.LogWarning("Connection was broken. Attempting to close and then re-open");
                    conn.Close();
                    await conn.OpenWithRetryAsync();
                    break;
                }
            }
        }

        /// <summary>
        /// Logs that a polly retry will be processed (if retry count not exceeded)
        /// </summary>
        /// <param name="exception">Exception that was raised</param>
        /// <param name="sleepDuration">Timespan before the operation will be retried</param>
        /// <param name="retryCount">The number of times that this operation has been retried</param>
        /// <param name="contextData">Polly context data</param>
        /// <returns></returns>
        private static Task LogPollyRetry(Exception exception, TimeSpan sleepDuration, int retryCount, Context contextData)
        {
            var additionalLogData = string.Empty;
            if (exception is SqlException sqlException)
            {
                var sqlErrors = sqlException.Errors.OfType<SqlError>().ToList();
                additionalLogData = $" : SQL Error(s) : {string.Join(",", sqlErrors.Select(e => $"{e.Number}"))}";
            }

            var retryMessage = retryCount >= SqlRetryCount ? "Retries exhausted" : $"Will retry after {sleepDuration.TotalSeconds}s";
            _logger.LogWarning(exception, $"{contextData.PolicyKey}/{contextData.OperationKey} failed {retryCount} time(s) for polly id({contextData.CorrelationId}). {retryMessage}{additionalLogData}");
            return Task.CompletedTask;
        }
    }
}
