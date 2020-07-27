import os
import uuid

from _BlobTest import _BlobTest

from azure.test.perfstress import RandomStream
from azure.test.perfstress import AsyncRandomStream

class StageBlockTest(_BlobTest):
    block_id = str(uuid.uuid4())

    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        self.data = b'a' * self.Arguments.size

    def Run(self):
        data = RandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        self.blob_client.stage_block(self.block_id, data, length=self.Arguments.size)

    async def RunAsync(self):
        data = AsyncRandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        await self.async_blob_client.stage_block(self.block_id, data, length=self.Arguments.size)

    @staticmethod
    def AddArguments(parser):
        super(StageBlockTest, StageBlockTest).AddArguments(parser)
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to upload.  Default is 10240.', default=10240)
        parser.add_argument('--stream', action='store_true', help='Upload stream instead of byte array.', default=False)
