import aiohttp
import argparse
import asyncio
import timeit

requestsSent = 0

async def fetch(session, url):
    async with session.get(url) as response:
        return await response.text()

async def fetchLoop(session, url, count):
    global requestsSent
    while True:
        if (requestsSent < count):
            requestsSent += 1
            await fetch(session, url)
        else:
            break

async def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("-c", "--count", type=int, default=10000)
    parser.add_argument("-p", "--parallel", type=int, default=1)
    parser.add_argument("-u", "--url", required=True)
    args = parser.parse_args()

    async with aiohttp.ClientSession() as session:
        # warmup
        await fetch(session, args.url)

        start = timeit.default_timer()
        await asyncio.gather(*[fetchLoop(session, args.url, args.count) for i in range(0, args.parallel)])
        elapsed = timeit.default_timer() - start
        ops_per_second = int(args.count) / elapsed
        print(f"Processed {args.count} requests in {elapsed:.2f} seconds ({ops_per_second:.2f} RPS)")

if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main())
