using Azure.Messaging.ServiceBus;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PerfStressDriver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Undefined environment variable SERVICE_BUS_CONNECTION_STRING");
            }

            var client = new ServiceBusClient(connectionString);
            var processor = client.GetProcessor("jobs", new ServiceBusProcessorOptions
            {
                AutoComplete = true,
                MaxConcurrentCalls = 1,
            });

            processor.ProcessMessageAsync += async args =>
            {
                var message = args.Message;
                try
                {
                    var devOpsMessage = new DevOpsServiceBusMessage(message);

                    Console.WriteLine(devOpsMessage);

                    // Provision resource group

                    await devOpsMessage.SignalCompletion(succeeded: true);
                    await args.Receiver.CompleteAsync(message);
                }
                catch
                {
                    await args.Receiver.AbandonAsync(message);
                }
            };

            processor.ProcessErrorAsync += args =>
            {
                throw args.Exception;
            };

            await processor.StartProcessingAsync();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

    }
}
