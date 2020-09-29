using Azure.Messaging.EventHubs.PerfStress.Core;
using Azure.Test.PerfStress;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress
{
    public class EventProcessorLatencyTest : ProducerProcessorTest<PerfStressOptions>
    {
        private readonly SemaphoreSlim _receiveSemaphore = new SemaphoreSlim(0);

        private readonly SemaphoreSlim _errorSemaphore = new SemaphoreSlim(0);
        private Exception _exception;

        public EventProcessorLatencyTest(PerfStressOptions options) : base(options)
        {
            Processor.ProcessEventAsync += e =>
            {
                if (!e.CancellationToken.IsCancellationRequested)
                {
                    _receiveSemaphore.Release();
                }

                return Task.CompletedTask;
            };

            Processor.ProcessErrorAsync += e =>
            {
                if (!e.CancellationToken.IsCancellationRequested)
                {
                    _exception = e.Exception;
                    _errorSemaphore.Release();
                }

                return Task.CompletedTask;
            };
        }

        public override async Task SetupAsync()
        {
            await base.SetupAsync();
            await Processor.StartProcessingAsync();
        }

        public override void Run(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            using var batch = await Producer.CreateBatchAsync(cancellationToken);
            batch.TryAdd(new EventData(null));
            await Producer.SendAsync(batch, cancellationToken);

            var receiveTask = _receiveSemaphore.WaitAsync(cancellationToken);
            var errorTask = _errorSemaphore.WaitAsync(cancellationToken);
            var completedTask = await Task.WhenAny(receiveTask, errorTask);

            if (completedTask == errorTask)
            {
                throw _exception;
            }
        }
    }
}
