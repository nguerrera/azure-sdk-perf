using Azure.Storage.Blobs.PerfStress.Core;
using Azure.Test.PerfStress;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs.PerfStress
{
    public class StageBlockTest : RandomBlobTest<StorageTransferOptionsOptions>
    {
        private readonly string _blockId = Guid.NewGuid().ToString("N");

        public StageBlockTest(StorageTransferOptionsOptions options) : base(options)
        {
        }

        public override void Run(CancellationToken cancellationToken)
        {
            using var stream = RandomStream.Create(Options.Size);
            BlockBlobClient.StageBlock(_blockId, stream);
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            using var stream = RandomStream.Create(Options.Size);
            await BlockBlobClient.StageBlockAsync(_blockId, stream);
        }
    }
}
