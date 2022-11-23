using CodeSniffer.Core.Sniffer;
using Serilog;
using Sniffer.DotNetversion;
using Sniffer.GitStaleBranches;
using Sniffer.VueI18n;

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
                //var sniffer = new DotNetVersionSniffer(logger, DotNetVersionOptions.Default());
                //var sniffer = new GitStaleBranchesSniffer(logger, GitStaleBranchesOptions.Default());
                var sniffer = new VueI18NSniffer(logger, VueI18NOptions.Default());

                report = await sniffer.Execute(
                    "D:\\temp",
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

                if (!string.IsNullOrEmpty(asset.Output))
                    Console.WriteLine(@"  " + asset.Output);

                Console.WriteLine();
            }
        }
    }

    internal class TestScanContext : ICsScanContext
    {
        public string BranchName => "master";
    }
}