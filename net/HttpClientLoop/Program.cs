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

            var sw = Stopwatch.StartNew();
            if (options.Sync)
            {
                var threads = new Thread[options.Parallel];

                for (var i = 0; i < options.Parallel; i++)
                {
                    var j = i;
                    threads[i] = new Thread(() => GetLoopAsync(options).Wait());
                    threads[i].Start();
                }
                for (var i = 0; i < options.Parallel; i++)
                {
                    threads[i].Join();
                }
            }
            else
            {
                var tasks = new Task[options.Parallel];
                for (var i = 0; i < options.Parallel; i++)
                {
                    tasks[i] = GetLoopAsync(options);
                }
                await Task.WhenAll(tasks);
            }
            sw.Stop();

            var elapsed = sw.Elapsed.TotalSeconds;
            var opsPerSecond = options.Count / elapsed;

            Console.WriteLine($"Processed {options.Count} requests in {elapsed:N2} seconds ({opsPerSecond:N2} RPS)");
        }

        private static async Task GetLoopAsync(Options options)
        {
            while (true)
            {
                if (Interlocked.Increment(ref _requestsSent) <= options.Count)
                {
                    var task = _httpClient.GetStringAsync(options.Url);
                    if (options.Sync)
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
