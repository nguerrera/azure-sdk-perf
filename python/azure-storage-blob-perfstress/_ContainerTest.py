import uuid
from _ServiceTest import _ServiceTest

class _ContainerTest(_ServiceTest):
    container_name = "perfstress-" + str(uuid.uuid4())

    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        self.container_client = self.service_client.get_container_client(type(self).container_name)
        self.async_container_client = self.async_service_client.get_container_client(type(self).container_name)
        await self.async_container_client.__aenter__()
        self.container_client.create_container()

    async def GlobalCleanupAsync(self):
        self.container_client.delete_container()
        await self.async_container_client.__aexit__()
        await super().GlobalCleanupAsync()

