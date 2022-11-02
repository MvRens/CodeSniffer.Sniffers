using CodeSniffer.Core.Sniffer;
using Serilog;
using Sniffer.DotNetversion;

namespace SniffersTest
{
    public class Program
    {
        public static async Task Main()
        {
            ICsReport? report;

            await using (var logger = new LoggerConfiguration()
                             .WriteTo.Console()
                             .CreateLogger())
            {
                var sniffer = new DotNetVersionSniffer(logger, DotNetVersionOptions.Default());
                report = await sniffer.Execute(
                    "C:\\path\\to\\test",
                    new TestScanContext(),
                    CancellationToken.None);
            }

            if (report == null)
                return;

            Console.WriteLine();

            foreach (var asset in report.Assets)
            {
                Console.WriteLine(asset.Name);
                Console.WriteLine(@"  " + asset.Summary);
                Console.WriteLine();
            }
        }
    }

    internal class TestScanContext : ICsScanContext
    {
        public string BranchName => "master";
    }
}