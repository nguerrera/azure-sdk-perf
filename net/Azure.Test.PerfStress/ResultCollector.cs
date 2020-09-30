using System;

namespace Azure.Test.PerfStress
{
    public abstract class ResultCollector
    {
        public void Add()
        {
            Add(TimeSpan.Zero);
        }

        public void Add(TimeSpan latency)
        {
            Add(latency, latency);
        }

        public abstract void Add(TimeSpan latency, TimeSpan correctedLatency);

        public void Increment(string metric)
        {
            Increment(metric, 1);
        }

        public abstract void Increment(string metric, long value);
    }
}
