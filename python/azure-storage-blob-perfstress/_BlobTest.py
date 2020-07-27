import uuid
from _ContainerTest import _ContainerTest

class _BlobTest(_ContainerTest):
    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        blob_name = "blobtest-" + str(uuid.uuid4())
        self.blob_client = self.container_client.get_blob_client(blob_name)
        self.async_blob_client = self.async_container_client.get_blob_client(blob_name)
        await self.async_blob_client.__aenter__()

    async def GlobalCleanupAsync(self):
        await self.async_blob_client.__aexit__()
        await super().GlobalCleanupAsync()

