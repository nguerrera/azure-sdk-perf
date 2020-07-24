using Azure.Storage.Blobs.PerfStress.Core;
using Azure.Test.PerfStress;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs.PerfStress
{
    public class UploadTest : BlobTest<StorageTransferOptionsOptions>
    {
        public UploadTest(StorageTransferOptionsOptions options) : base(options)
        {
        }

        public override void Run(CancellationToken cancellationToken)
        {
            using var stream = RandomStream.Create(Options.Size);
            BlobClient.Upload(stream, transferOptions: Options.StorageTransferOptions, cancellationToken: cancellationToken);
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            using var stream = RandomStream.Create(Options.Size);
            await BlobClient.UploadAsync(stream, transferOptions: Options.StorageTransferOptions,  cancellationToken: cancellationToken);
        }
    }
}
