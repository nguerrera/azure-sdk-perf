import os
import uuid

from azure.test.perfstress import PerfStressTest
from azure.test.perfstress import RandomStream
from azure.test.perfstress import AsyncRandomStream

from azure.storage.blob import ContainerClient as SyncContainerClient
from azure.storage.blob.aio import ContainerClient as AsyncContainerClient

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

        if self.Arguments.max_single_put_size:
            type(self).container_client = SyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name, max_single_put_size=self.Arguments.max_single_put_size)
            type(self).async_container_client = AsyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name, max_single_put_size=self.Arguments.max_single_put_size)
        else:
            type(self).container_client = SyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name)
            type(self).async_container_client = AsyncContainerClient.from_connection_string(conn_str=connection_string, container_name=self.container_name)

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
        data = RandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        self.blob_client.stage_block(self.block_id, data, length=self.Arguments.size)

    async def RunAsync(self):
        data = AsyncRandomStream(self.Arguments.size) if self.Arguments.stream else self.data
        await self.async_blob_client.stage_block(self.block_id, data, length=self.Arguments.size)

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('--max-single-put-size', nargs='?', type=int, help='Maximum size of blob uploading in single HTTP PUT.  Default is None.')
        parser.add_argument('-s', '--size', nargs='?', type=int, help='Size of blobs to upload.  Default is 10240.', default=10240)
        parser.add_argument('--stream', action='store_true', help='Upload stream instead of byte array.', default=False)
