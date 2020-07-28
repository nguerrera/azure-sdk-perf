import os

from _ContainerTest import _ContainerTest

class DownloadBlobTest(_ContainerTest):
    def __init__(self, arguments):
        super().__init__(arguments)
        blob_name = "downloadtest"
        self.blob_client = self.container_client.get_blob_client(blob_name)
        self.async_blob_client = self.async_container_client.get_blob_client(blob_name)

    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        data = b'a' * self.Arguments.size
        self.blob_client.upload_blob(data)

    async def CleanupAsync(self):
        await self.async_blob_client.close()
        await super().CleanupAsync()

    def Run(self):
        self.blob_client.download_blob().readall()

    async def RunAsync(self):
        stream = await self.async_blob_client.download_blob()
        await stream.readall()

    @staticmethod
    def AddArguments(parser):
        super(DownloadBlobTest, DownloadBlobTest).AddArguments(parser)
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to download.  Default is 10240.', default=10240)
