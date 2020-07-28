import os

from _BlobTest import _BlobTest

from azure.test.perfstress import RandomStream
from azure.test.perfstress import AsyncRandomStream

class UploadBlobTest(_BlobTest):
    def __init__(self, arguments):
        super().__init__(arguments)
        self.data = b'a' * self.Arguments.size

    def Run(self):
        data = RandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        self.blob_client.upload_blob(data, length=self.Arguments.size, overwrite=True)

    async def RunAsync(self):
        data = AsyncRandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        await self.async_blob_client.upload_blob(data, length=self.Arguments.size, overwrite=True)

    @staticmethod
    def AddArguments(parser):
        super(UploadBlobTest, UploadBlobTest).AddArguments(parser)
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to upload.  Default is 10240.', default=10240)
        parser.add_argument('--stream', action='store_true', help='Upload stream instead of byte array.', default=False)
