using CommandLine;

namespace Azure.Test.PerfStress
{
    public class UrlOptions : PerfStressOptions
    {
        [Option('u', "url", Required = true)]
        public string Url { get; set; }
    }
}
