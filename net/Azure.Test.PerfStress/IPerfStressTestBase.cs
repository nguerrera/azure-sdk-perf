using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Test.PerfStress
{
    internal interface IPerfStressTestBase : IDisposable, IAsyncDisposable
    {
        Task GlobalSetupAsync();
        Task SetupAsync();
        void Run(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken);
        Task RunAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken);
        Task CleanupAsync();
        Task GlobalCleanupAsync();
    }
}
