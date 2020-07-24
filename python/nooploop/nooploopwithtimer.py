import argparse
import time

def noop():
    return None

parser = argparse.ArgumentParser()
parser.add_argument("-c", "--count", default=1000000)
args = parser.parse_args()

duration = 5
completed_operations = [0] * 1
last_completion_times = [0] * 1

id = 0
start = time.time()
runtime = 0
for x in range(int(args.count)):
    noop()
    runtime = time.time() - start
    # completed_operations[id] += 1
    # last_completion_times[id] = runtime

# elapsed = last_completion_times[0]
# ops_per_second = completed_operations[0] / elapsed

# print(f"Called {completed_operations[0]} functions in {elapsed:.2f} seconds ({ops_per_second:.2f} ops/s)")

print(runtime)
