using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Xigadee
{
    /// <summary>
    /// This class is not currently supported.
    /// </summary>
    [Obsolete("This class is a placeholder and is not currently supported.")]
    public class AzureStorageConnectorFile: AzureStorageConnectorBase<FileRequestOptions, BinaryContainer>
    {
        /// <summary>
        /// This is the file client.
        /// </summary>
        public CloudFileClient Client { get; set; }
        /// <summary>
        /// This method writes to the incoming event to the underlying storage technology.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="id">The microservice metadata.</param>
        /// <returns>
        /// This is an async task.
        /// </returns>
        /// <exception cref="NotImplementedException">Not currently supported.</exception>
        public override Task Write(EventHolder e, MicroserviceId id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        /// <exception cref="NotImplementedException">Not currently supported.</exception>
        public override void Initialize()
        {
            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            throw new NotImplementedException();
        }
        /// <summary>
        /// This method is used to check that the specific event should be written to the underlying storage.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>
        /// Returns true if the event should be written. Currently always returns false;
        /// </returns>
        public override bool ShouldWrite(EventHolder e)
        {
            return false;
            //return Options.IsSupported(AzureStorageBehaviour.File, e);
        }
    }
}
