using Azure.Messaging.EventHubs.PerfStress.Core;
using Azure.Test.PerfStress;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress
{
    public class EventProcessorTest : ProducerProcessorTestBase<PerfStressOptions>
    {
        public EventProcessorTest(PerfStressOptions options) : base(options)
        {
        }

        public override void Run(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // TODO
        // - Ensure message is not left in queue no matter when method is cancelled
        public override async Task RunAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

            Processor.ProcessEventAsync += e =>
            {
                var end = sw.ElapsedTicks;

                if (e.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                var start = BitConverter.ToInt64(e.Data.Body.ToArray(), 0);

                resultCollector.Add(TimeSpan.FromTicks(end - start));

                // Allow producer to send next message
                semaphore.Release();

                return Task.CompletedTask;
            };

            await Processor.StartProcessingAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                // Don't send a new message until processor has handled the previous message
                await semaphore.WaitAsync(cancellationToken);

                var start = sw.ElapsedTicks;
                using (var batch = await Producer.CreateBatchAsync(cancellationToken))
                {
                    batch.TryAdd(new EventData(BitConverter.GetBytes(start)));
                    await Producer.SendAsync(batch, cancellationToken);
                }
            }

            // Do not flow cancellationToken, since we want processing to stop gracefully even after the test run is cancelled
            await Processor.StopProcessingAsync();
        }
    }
}
