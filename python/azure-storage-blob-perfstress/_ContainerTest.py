import uuid
from _ServiceTest import _ServiceTest

class _ContainerTest(_ServiceTest):
    container_name = "perfstress-" + str(uuid.uuid4())

    def __init__(self, arguments):
        super().__init__(arguments)
        self.container_client = self.service_client.get_container_client(type(self).container_name)
        self.async_container_client = self.async_service_client.get_container_client(type(self).container_name)

    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        self.container_client.create_container()

    async def SetupAsync(self):
        await super().SetupAsync()
        await self.async_container_client.__aenter__()

    async def GlobalCleanupAsync(self):
        self.container_client.delete_container()
        await super().GlobalCleanupAsync()

    async def CleanupAsync(self):
        await self.async_container_client.__aexit__()
        await super().CleanupAsync()
