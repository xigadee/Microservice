using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    public class TransportConnectorMMB:ApiProviderBase
    {
        public TransportConnectorMMB(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            ) : base(uri, authHandlers, clientCert, manualCertValidation)
        {
        }


        ///// <summary>
        ///// Creates a batch for the ApiUser to mark transactions against.
        ///// </summary>
        ///// <param name="batch">The new batch.</param>
        ///// <returns>Returns a response with the batch response object.</returns>
        ///// <exception cref="ArgumentNullException">request: the TransactionV2BatchRequest object cannot be null.</exception>
        //public Task<RsWrapper<TransactionV2Batch>> BatchCreate(TransactionV2Batch batch)
        //    => CallClient<TransactionV2Batch>(HttpMethod.Post, cnBatchRoot, batch ?? throw new ArgumentNullException(nameof(batch), " batch object cannot be null."));

        ///// <summary>
        ///// Reads the current batch status.
        ///// </summary>
        ///// <param name="id">The optional web hook identifier. If this is not passed all registered web hooks will be returned.</param>
        ///// <returns>Returns a response with the batch response object.</returns>
        //public Task<RsWrapper<TransactionV2Batch>> BatchRead(Guid id)
        //    => CallClient<TransactionV2Batch>(HttpMethod.Get, $"{cnBatchRoot}/{id:N}");

        ///// <summary>
        ///// Marks the batch as cancelled.
        ///// </summary>
        ///// <param name="id">The batch identifier.</param>
        ///// <param name="status">The optional batch update request status.</param>
        ///// <returns>Returns a response with the batch response object.</returns>
        //public Task<RsWrapper<TransactionV2Batch>> BatchCancel(Guid id, BatchUpdateRequest status = null)
        //    => CallClient<TransactionV2Batch>(HttpMethod.Post, $"{cnBatchRoot}/Cancel/{id:N}", status);

        ///// <summary>
        ///// Marks the batch as complete.
        ///// </summary>
        ///// <param name="id">The batch identifier.</param>
        ///// <param name="status">The optional batch update request status.</param>
        ///// <returns>Returns a response with the batch response object.</returns>
        //public Task<RsWrapper<TransactionV2Batch>> BatchComplete(Guid id, BatchUpdateRequest status = null)
        //    => CallClient<TransactionV2Batch>(HttpMethod.Post, $"{cnBatchRoot}/Complete/{id:N}", status);

        ///// <summary>
        ///// Marks the batch as in error, and to be investigated manually.
        ///// </summary>
        ///// <param name="id">The batch identifier.</param>
        ///// <param name="status">The optional batch update request status.</param>
        ///// <returns>Returns a response with the batch response object.</returns>
        //public Task<RsWrapper<TransactionV2Batch>> BatchError(Guid id, BatchUpdateRequest status = null)
        //    => CallClient<TransactionV2Batch>(HttpMethod.Post, $"{cnBatchRoot}/Error/{id:N}", status);
    }
}
