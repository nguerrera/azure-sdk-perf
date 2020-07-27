import os

from _BlobTest import _BlobTest

class DownloadBlobTest(_BlobTest):
    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        data = b'a' * self.Arguments.size
        self.blob_client.upload_blob(data)

    def Run(self):
        self.blob_client.download_blob().readall()

    async def RunAsync(self):
        stream = await self.async_blob_client.download_blob()
        await stream.readall()

    @staticmethod
    def AddArguments(parser):
        super(DownloadBlobTest, DownloadBlobTest).AddArguments(parser)
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to download.  Default is 10240.', default=10240)
