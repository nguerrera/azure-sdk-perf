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

        public override async Task RunAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            
            using var sendSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

            // TODO: Assign event handlers to local variables to allow detaching
            Processor.ProcessEventAsync += e =>
            {
                var end = sw.ElapsedTicks;

                if (e.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                var start = BitConverter.ToInt64(e.Data.Body.ToArray(), 0);

                Console.WriteLine($"Start: {start}, End: {end}");

                resultCollector.Add(latency: TimeSpan.FromTicks(end - start));

                // Allow producer to send next message
                sendSemaphore.Release();

                return Task.CompletedTask;
            };

            Processor.ProcessErrorAsync += e =>
            {
                throw e.Exception;
            };

            try
            {
                Console.WriteLine("Producer.StartProcessingAsync()");

                // TODO: If event hub has multiple partitions, the first event may not be processed for many seconds
                await Processor.StartProcessingAsync(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("sendSemaphore.WaitAsync()");
                    // Don't send a new message until processor has received the previous message.
                    await sendSemaphore.WaitAsync(cancellationToken);

                    try
                    {
                        Console.WriteLine("Producer.CreateBatchAsync()");
                        using var batch = await Producer.CreateBatchAsync(cancellationToken);

                        var start = sw.ElapsedTicks;
                        batch.TryAdd(new EventData(BitConverter.GetBytes(start)));

                        Console.WriteLine("Producer.SendAsync()");
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
                // TODO: Detach event handlers

                // TODO: StopProcessingAsync() takes about 30 seconds (!) to complete

                //Console.WriteLine("await Producer.StopProcessingAsync()");
                //// Do not flow cancellationToken, since we want processing to stop gracefully even after the test run is cancelled
                //await Processor.StopProcessingAsync();
                //Console.WriteLine("awaited Producer.StopProcessingAsync()");
            }
        }
    }
}
