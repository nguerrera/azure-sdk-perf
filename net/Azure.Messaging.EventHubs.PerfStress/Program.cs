using Azure.Test.PerfStress;
using System.Threading.Tasks;

namespace Azure.Messaging.EventHubs.PerfStress
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await PerfStressProgram.Main(typeof(Program).Assembly, args);
        }
    }
}
