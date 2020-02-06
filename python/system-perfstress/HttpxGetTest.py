import httpx

from azure.test.perfstress import PerfStressTest

class HttpxGetTest(PerfStressTest):

    async def GlobalSetupAsync(self):
        type(self).client = httpx.AsyncClient()

    async def GlobalCleanupAsync(self):
        await type(self).client.aclose()

    async def RunAsync(self):
        response = await type(self).client.get(self.Arguments.url)
        body = response.text

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-u', '--url', required=True)
