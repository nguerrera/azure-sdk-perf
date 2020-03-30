const { program } = require('commander');

const NS_PER_SEC = 1e9;

function noop() {
}

async function noopasync() {
}

(async () => {
  program
    .option('-c, --count <number>', 'count of function calls', 1000000)
    .option('--sync', 'Runs sync version of test')

  program.parse(process.argv)

  const start = process.hrtime();

  if (program.sync) {
    for (i = 0; i < program.count; i++) {
      noop();
    }
  }
  else {
    for (i = 0; i < program.count; i++) {
      await noopasync();
    }
  }

  const elapsed = process.hrtime(start);
  const elapsedSeconds = elapsed[0] + (elapsed[1] / NS_PER_SEC);
  const opsPerSecond = program.count / elapsedSeconds;

  // print(f"Called {args.count} functions in {elapsed:.2f} seconds ({ops_per_second:.2f} ops/s)")

  console.log("Called %i functions in %f seconds (%f ops/s)", program.count, elapsedSeconds.toFixed(2), opsPerSecond.toFixed(2));
})();