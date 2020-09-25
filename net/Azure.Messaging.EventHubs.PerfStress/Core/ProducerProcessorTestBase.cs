using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs;
using Azure.Test.PerfStress;
using System;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress.Core
{
    public abstract class ProducerProcessorTestBase<TOptions> : ProducerTestBase<TOptions> where TOptions : PerfStressOptions
    {
        protected string StorageConnectionString { get; private set; }
        protected string BlobContainerName { get; private set; }
        protected EventProcessorClient Processor { get; private set; }

        protected ProducerProcessorTestBase(TOptions options) : base(options)
        {
            StorageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
            if (string.IsNullOrEmpty(StorageConnectionString))
            {
                throw new InvalidOperationException("Undefined environment variable STORAGE_CONNECTION_STRING");
            }

            BlobContainerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME");
            if (string.IsNullOrEmpty(BlobContainerName))
            {
                throw new InvalidOperationException("Undefined environment variable BLOB_CONTAINER_NAME");
            }

            var checkpointStore = new BlobContainerClient(StorageConnectionString, BlobContainerName);
            Processor = new EventProcessorClient(checkpointStore, EventHubConsumerClient.DefaultConsumerGroupName, EventHubsConnectionString, EventHubName);
        }
    }
}
