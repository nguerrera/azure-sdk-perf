import os

from azure.test.perfstress import PerfStressTest

from azure.storage.blob import BlobServiceClient as SyncBlobServiceClient
from azure.storage.blob.aio import BlobServiceClient as AsyncBlobServiceClient

class _ServiceTest(PerfStressTest):
    service_client = None
    async_service_client = None

    def __init__(self, arguments):
        super().__init__(arguments)

        connection_string = os.environ.get("STORAGE_CONNECTION_STRING")
        if not connection_string:
            raise Exception("Undefined environment variable STORAGE_CONNECTION_STRING")

        if not _ServiceTest.service_client or self.Arguments.service_client_per_instance:
            if self.Arguments.max_single_put_size:
                _ServiceTest.service_client = SyncBlobServiceClient.from_connection_string(conn_str=connection_string, max_single_put_size=self.Arguments.max_single_put_size)
                _ServiceTest.async_service_client = AsyncBlobServiceClient.from_connection_string(conn_str=connection_string, max_single_put_size=self.Arguments.max_single_put_size)
            else:
                _ServiceTest.service_client = SyncBlobServiceClient.from_connection_string(conn_str=connection_string)
                _ServiceTest.async_service_client = AsyncBlobServiceClient.from_connection_string(conn_str=connection_string)

        self.service_client = _ServiceTest.service_client
        self.async_service_client =_ServiceTest.async_service_client

    async def CloseAsync(self):
        await self.async_service_client.close()
        await super().CloseAsync()

    @staticmethod
    def AddArguments(parser):
        super(_ServiceTest, _ServiceTest).AddArguments(parser)
        parser.add_argument('--max-single-put-size', nargs='?', type=int, help='Maximum size of blob uploading in single HTTP PUT.  Default is None.')
        parser.add_argument('--service-client-per-instance', action='store_true', help='Create one ServiceClient per test instance.  Default is to share a single ServiceClient.', default=False)
