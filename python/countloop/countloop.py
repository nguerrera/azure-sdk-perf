import argparse
import timeit
import time

def noop():
    pass

def time_time():
    noop()
    foo = time.time()

start = time.time()

def time_time_subtract():
    noop()
    foo = start - time.time()

completed_operations_array = [0] * 1

def time_time_subtract_increment_array():
    global completed_operations_array
    noop()
    foo = start - time.time()
    completed_operations_array[0] += 1

completed_operations_int = 0

def time_time_subtract_increment_int():
    global completed_operations_int
    noop()
    foo = start - time.time()
    completed_operations_int += 1

parser = argparse.ArgumentParser()
parser.add_argument("-c", "--count", default=1000000)
args = parser.parse_args()

start = timeit.default_timer()

for x in range(int(args.count)):
    time_time_subtract_increment_array()

elapsed = timeit.default_timer() - start
ops_per_second = int(args.count) / elapsed

print(f"Called {args.count} functions in {elapsed:.2f} seconds ({ops_per_second:.2f} ops/s)")

