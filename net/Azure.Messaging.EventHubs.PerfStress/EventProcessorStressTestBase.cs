using Azure.Messaging.EventHubs.PerfStress.Core;
using Azure.Test.PerfStress;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress
{
    public class EventProcessorStressTestBase : ProducerProcessorTestBase<PerfStressOptions>
    {
        public EventProcessorStressTestBase(PerfStressOptions options) : base(options)
        {
        }

        public override void Run(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task RunAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
