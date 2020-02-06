import asyncio
from urllib.parse import urlparse

from azure.test.perfstress import PerfStressTest

class SocketHttpGetTest(PerfStressTest):

    async def SetupAsync(self):
        parsedUrl = urlparse(self.Arguments.url)
        hostname = parsedUrl.hostname
        port = parsedUrl.port
        path = parsedUrl.path

        message = f'GET {path} HTTP/1.1\r\nHost: {hostname}:{port}\r\n\r\n'
        self.messageBytes = message.encode()

        self.reader, self.writer = await asyncio.open_connection(parsedUrl.hostname, parsedUrl.port)
 
    async def CleanupAsync(self):
        self.writer.close()

    async def RunAsync(self):
        self.writer.write(self.messageBytes)
        await self.reader.read(200)

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-u', '--url', required=True)
