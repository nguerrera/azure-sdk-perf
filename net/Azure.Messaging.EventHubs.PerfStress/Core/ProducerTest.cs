using Azure.Messaging.EventHubs.Producer;
using Azure.Test.PerfStress;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress.Core
{
    public abstract class ProducerTest<TOptions> : PerfStressTest<TOptions> where TOptions : PerfStressOptions
    {
        protected string EventHubsConnectionString { get; private set; }
        protected string EventHubName { get; private set; }
        protected EventHubProducerClient Producer { get; private set; }

        public ProducerTest(TOptions options) : base(options)
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

            Producer = new EventHubProducerClient(EventHubsConnectionString, EventHubName);
        }

        public override async ValueTask DisposeAsyncCore()
        {
            if (Producer != null)
            {
                await Producer.DisposeAsync();
            }

            Producer = null;

            await base.DisposeAsyncCore();
        }
    }
}
