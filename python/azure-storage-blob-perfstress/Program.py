import asyncio
import logging

from azure.test.perfstress import PerfStressRunner

if __name__ == '__main__':  
    main_loop = PerfStressRunner()
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main_loop.RunAsync())
