import requests

from core.PerfStressTest import PerfStressTest

class RequestsHttpClientGetTest(PerfStressTest):

    async def GlobalSetupAsync(self):
        type(self).session = requests.Session()

    def Run(self):
        type(self).session.get(self.Arguments.url).text

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-u', '--url', required=True)
