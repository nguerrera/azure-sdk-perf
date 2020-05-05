import os
import uuid
from io import BytesIO

from azure.test.perfstress import PerfStressTest

from azure.storage.blob import ContainerClient as SyncContainerClient
from azure.storage.blob.aio import ContainerClient as AsyncContainerClient

class LargeStream(BytesIO):
    def __init__(self, length, initial_buffer_length=1024 * 1024):
        super().__init__()
        self._base_data = os.urandom(initial_buffer_length)
        self._base_data_length = initial_buffer_length
        self._position = 0
        self._remaining = length
        self._closed = False

    def read(self, size=None):
        if self._remaining == 0:
            return b""

        if size is None:
            e = self._base_data_length
        else:
            e = size
        e = min(e, self._remaining)
        if e > self._base_data_length:
            self._base_data = os.urandom(e)
            self._base_data_length = e
        self._remaining = self._remaining - e
        return self._base_data[:e]

    def remaining(self):
        return self._remaining

    def close(self):
        self._closed = True

class StageBlockTest(PerfStressTest):
    container_name = PerfStressTest.NewGuid()
    blob_name = PerfStressTest.NewGuid()
    block_id = str(uuid.uuid4())
    data = None
    blob_client = None
    async_blob_client = None

    async def GlobalSetupAsync(self):
        connection_string = os.environ.get("STORAGE_CONNECTION_STRING")
        if not connection_string:
            raise Exception("Undefined environment variable STORAGE_CONNECTION_STRING")

        type(self).container_client = SyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name, max_single_put_size=256*1024*1024)
        
        type(self).async_container_client = AsyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name, max_single_put_size=256*1024*1024)
        await type(self).async_container_client.__aenter__()

        type(self).container_client.create_container()

        self.blob_client = type(self).container_client.get_blob_client(self.blob_name)

        self.async_blob_client = type(self).async_container_client.get_blob_client(self.blob_name)
        await self.async_blob_client.__aenter__()

        self.data = b'a' * self.Arguments.size


    async def GlobalCleanupAsync(self):
        type(self).container_client.delete_container()

        await type(self).async_container_client.__aexit__()


    def Run(self):
        self.blob_client.stage_block(self.block_id, LargeStream(self.Arguments.size), length=self.Arguments.size)

    async def RunAsync(self):
        await type(self).async_blob_client.stage_block(self.block_id, LargeStream(self.Arguments.size), length=self.Arguments.size)

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to upload.  Default is 10240.', default=10240)
