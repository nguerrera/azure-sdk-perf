using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PerfStressDriver
{
    public static class ProcessUtil
    {
        public static ProcessResult Run(string fileName, string arguments = null, Action<string> outputDataReceived = null, Action<string> errorDataReceived = null)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            using var process = new Process()
            {
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                },
            };

            process.OutputDataReceived += (_, e) =>
            {
                // e.Data is null to signal end of stream
                if (e.Data != null)
                {
                    outputDataReceived?.Invoke(e.Data);
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                // e.Data is null to signal end of stream
                if (e.Data != null)
                {
                    errorDataReceived?.Invoke(e.Data);
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            return new ProcessResult(process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
        }
    }
}
