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
            
            using var sendSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

            Processor.ProcessEventAsync += e =>
            {
                var end = sw.ElapsedTicks;

                if (e.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                var start = BitConverter.ToInt64(e.Data.Body.ToArray(), 0);
                resultCollector.Add(latency: TimeSpan.FromTicks(end - start));

                // Allow producer to send next message
                sendSemaphore.Release();

                return Task.CompletedTask;
            };

            try
            {
                await Processor.StartProcessingAsync(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Don't send a new message until processor has received the previous message.
                    await sendSemaphore.WaitAsync(cancellationToken);

                    try
                    {
                        using var batch = await Producer.CreateBatchAsync(cancellationToken);

                        var start = sw.ElapsedTicks;
                        batch.TryAdd(new EventData(BitConverter.GetBytes(start)));

                        await Producer.SendAsync(batch, cancellationToken);
                    }
                    catch
                    {
                        // Message was not actually sent, so reset sendSemaphore to allow test to terminate.
                        sendSemaphore.Release();
                        throw;
                    }
                }

                // We never want to end test with a message in the queue (even if the test is cancelled), so we wait until the last
                // sent message has been received (and do not flow cancellationToken).
                await sendSemaphore.WaitAsync();
            }
            finally
            {
                // Do not flow cancellationToken, since we want processing to stop gracefully even after the test run is cancelled
                await Processor.StopProcessingAsync();
            }
        }
    }
}
