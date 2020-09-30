using Azure.Messaging.EventHubs.PerfStress.Core;
using Azure.Messaging.EventHubs.Producer;
using Azure.Test.PerfStress;
using CommandLine;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress
{
    public class EventProcessorStressTestBase : PerfStressTestBase<EventProcessorStressTestBase.EventProcessorStressOptions>
    {
        protected string EventHubsConnectionString { get; private set; }
        protected string EventHubName { get; private set; }
        protected string StorageConnectionString { get; private set; }
        protected string BlobContainerName { get; private set; }

        public EventProcessorStressTestBase(EventProcessorStressOptions options) : base(options)
        {
            EventHubsConnectionString = Environment.GetEnvironmentVariable("EVENT_HUBS_CONNECTION_STRING");
            if (string.IsNullOrEmpty(EventHubsConnectionString))
            {
                throw new InvalidOperationException("Undefined environment variable EVENT_HUBS_CONNECTION_STRING");
            }

            EventHubName = Environment.GetEnvironmentVariable("EVENT_HUB_NAME");
            if (string.IsNullOrEmpty(EventHubName))
            {
                throw new InvalidOperationException("Undefined environment variable EVENT_HUB_NAME");
            }

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
        }

        public override void Run(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task RunAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var publishTask = Publish(resultCollector, cancellationToken);
            await publishTask;
        }

        private async Task Publish(ResultCollector resultCollector, CancellationToken cancellationToken)
        {
            var producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);

            while (!cancellationToken.IsCancellationRequested)
            {
                using var batch = await producer.CreateBatchAsync();

                for (var i = 0; i < Options.PublishBatchSize; i++)
                {
                    if (!batch.TryAdd(new EventData(null)))
                    {
                        break;
                    }
                }

                await producer.SendAsync(batch, cancellationToken);

                resultCollector.Increment("EventsPublished", batch.Count);
                resultCollector.Increment("TotalServiceOperations");

                await Task.Delay(TimeSpan.FromMilliseconds(Options.PublishingDelayMs));
            }
        }

        public class EventProcessorStressOptions : PerfStressOptions {
            [Option("publishBatchSize", Default = 1, HelpText = "Messages per published batch")]
            public int PublishBatchSize { get; set; }

            [Option("publishingDelayMs", Default = 500, HelpText = "Delay between batch publishes (in milliseconds)")]
            public int PublishingDelayMs { get; set; }
        }
    }
}
