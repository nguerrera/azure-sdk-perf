using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Azure.Test.PerfStress
{
    internal interface IPerfStressTestBase
    {
        Task GlobalSetupAsync();
        Task SetupAsync();
        void RunBase(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken);
        Task RunBaseAsync(ResultCollector resultCollector, bool latency, Channel<(TimeSpan, Stopwatch)> pendingOperations, CancellationToken cancellationToken);
        Task CleanupAsync();
        Task GlobalCleanupAsync();
    }
}
