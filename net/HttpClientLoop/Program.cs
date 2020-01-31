using CommandLine;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientLoop
{
    class Program
    {
        private static HttpClient _httpClient;
        private static long _requestsSent;

        public class Options
        {
            [Option('c', "count", Default = 10_000)]
            public long Count { get; set; }

            [Option("insecure", Default = false)]
            public bool Insecure { get; set; }

            [Option('p', "parallel", Default = 1)]
            public int Parallel { get; set; }

            [Option('s', "sync", Default = false)]
            public bool Sync { get; set; }

            [Option('u', "url", Required = true)]
            public string Url { get; set; }

            [Option('w', "warmup", Default = 5_000)]
            public long Warmup { get; set; }
        }

        static async Task Main(string[] args)
        {
            if (!GCSettings.IsServerGC)
            {
                throw new InvalidOperationException("Requires server GC");
            }

            await Parser.Default.ParseArguments<Options>(args).MapResult(
                async o => await Run(o),
                errors => Task.CompletedTask);
        }

        private static async Task Run(Options options)
        {
            if (options.Insecure)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                _httpClient = new HttpClient(httpClientHandler);
            }
            else
            {
                _httpClient = new HttpClient();
            }

            // Warmup
            Console.WriteLine("=== Warmup ===");
            await RunLoops(options.Url, options.Warmup, options.Sync, options.Parallel);

            _requestsSent = 0;

            Console.WriteLine();
            Console.WriteLine("=== Test ===");
            await RunLoops(options.Url, options.Count, options.Sync, options.Parallel);
        }

        private static async Task RunLoops(string url, long count, bool sync, int parallel)
        {
            var sw = Stopwatch.StartNew();
            if (sync)
            {
                var threads = new Thread[parallel];

                for (var i = 0; i < parallel; i++)
                {
                    var j = i;
                    threads[i] = new Thread(() => GetLoopAsync(url, count, sync).Wait());
                    threads[i].Start();
                }
                for (var i = 0; i < parallel; i++)
                {
                    threads[i].Join();
                }
            }
            else
            {
                var tasks = new Task[parallel];
                for (var i = 0; i < parallel; i++)
                {
                    tasks[i] = GetLoopAsync(url, count, sync);
                }
                await Task.WhenAll(tasks);
            }
            sw.Stop();

            var elapsed = sw.Elapsed.TotalSeconds;
            var opsPerSecond = count / elapsed;

            Console.WriteLine($"Processed {count} requests in {elapsed:N2} seconds ({opsPerSecond:N2} RPS)");
        }

        private static async Task GetLoopAsync(string url, long count, bool sync)
        {
            while (true)
            {
                if (Interlocked.Increment(ref _requestsSent) <= count)
                {
                    var task = _httpClient.GetStringAsync(url);
                    if (sync)
                    {
                        task.Wait();
                    }
                    else
                    {
                        await task;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _requestsSent);
                    break;
                }
            }
        }
    }
}
