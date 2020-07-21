using Azure.Core;
using Azure.Messaging.ServiceBus.PerfStress.Core;
using Azure.Test.PerfStress;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Messaging.ServiceBus.PerfStress
{
    public class SendReceiveTest : SenderReceiverTest<SizeOptions>
    {
        private readonly BinaryData _payload;

        public SendReceiveTest(SizeOptions options) : base(options)
        {
            _payload = BinaryData.FromStream(RandomStream.Create(options.Size));
        }

        public override void Run(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            await ServiceBusSender.SendMessageAsync(new ServiceBusMessage(_payload));
            var receivedMessage = await ServiceBusReceiver.ReceiveMessageAsync();
            await ServiceBusReceiver.CompleteMessageAsync(receivedMessage);
        }
    }
}
