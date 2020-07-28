import uuid

from _ContainerTest import _ContainerTest

class GetBlobsTest(_ContainerTest):
    async def GlobalSetupAsync(self):
        await super().GlobalSetupAsync()
        # TODO: Upload in parallel to improve setup perf
        for _ in range(0, self.Arguments.count): #pylint: disable=no-member
            self.container_client.upload_blob("getblobstest-" + str(uuid.uuid4()), '')

    def Run(self):
        for _ in self.container_client.list_blobs():
            pass

    async def RunAsync(self):
        async for _ in self.async_container_client.list_blobs(): #pylint: disable=not-an-iterable
            pass

    @staticmethod
    def AddArguments(parser):
        super(GetBlobsTest, GetBlobsTest).AddArguments(parser)
        parser.add_argument('-c', '--count', nargs='?', type=int, help='Number of blobs to populate.  Default is 1.', default=1)
