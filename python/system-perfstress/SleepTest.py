import math
import asyncio

from azure.test.perfstress import PerfStressTest

# Used for verifying the perf framework correctly computes average throughput across parallel tests of different speed
class SleepTest(PerfStressTest):
    instance_count = 0

    def __init__(self):
        type(self).instance_count += 1
        self.seconds_per_operation = math.pow(2, type(self).instance_count)

    def Run(self):
        time.sleep(self.seconds_per_operation)

    async def RunAsync(self):
        await asyncio.sleep(self.seconds_per_operation)
