using Azure.Storage.Blobs.Specialized;
using Azure.Test.PerfStress;
using System;

namespace Azure.Storage.Blobs.PerfStress.Core
{
    public abstract class BlobTest<TOptions> : ContainerTest<TOptions> where TOptions : SizeOptions
    {
        protected BlobClient BlobClient { get; private set; }
        protected BlockBlobClient BlockBlobClient { get; private set; }

        public BlobTest(TOptions options) : base(options)
        {
            var blobName = "blobtest-" + Guid.NewGuid();
            BlobClient = BlobContainerClient.GetBlobClient(blobName);
            BlockBlobClient = BlobContainerClient.GetBlockBlobClient(blobName);
        }
    }
}

