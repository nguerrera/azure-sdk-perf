using Azure.Test.PerfStress;
using System;

namespace Azure.Messaging.ServiceBus.PerfStress.Core
{
    public abstract class SenderReceiverTest<TOptions> : ServiceTest<TOptions> where TOptions : PerfStressOptions
    {
        protected ServiceBusSender ServiceBusSender { get; private set; }
        protected ServiceBusReceiver ServiceBusReceiver { get; private set; }

        public SenderReceiverTest(TOptions options) : base(options)
        {
            var queue = Environment.GetEnvironmentVariable("SERVICEBUS_QUEUE");

            if (string.IsNullOrEmpty(queue))
            {
                throw new InvalidOperationException("Undefined environment variable SERVICEBUS_QUEUE");
            }

            ServiceBusSender = ServiceBusClient.CreateSender(queue);
            ServiceBusReceiver = ServiceBusClient.CreateReceiver(queue);
        }
    }
}
