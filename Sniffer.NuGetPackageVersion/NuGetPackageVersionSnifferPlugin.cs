using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using JetBrains.Annotations;
using Serilog;

namespace Sniffer.NuGetPackageVersion
{
    [CsPlugin("b476b950-79db-4a1c-927c-49f16bc316b5", "NuGet Package Version")]
    [UsedImplicitly]
    public class NuGetPackageVersionSnifferPlugin : ICsSnifferPlugin, ICsPluginHelp
    {
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(NuGetPackageVersionOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var nugetpackageversionOptions = options.Deserialize<NuGetPackageVersionOptions>();
            CsOptionMissingException.ThrowIfNull(nugetpackageversionOptions);

            return new NuGetPackageVersionSniffer(logger, nugetpackageversionOptions);
        }


        public string GetOptionsHelpHtml(IReadOnlyList<CultureInfo> cultures)
        {
            var getString = Strings.ResourceManager.CreateGetString(cultures);

            return CsPluginHelpBuilder.Create()
                .SetSummary(getString(nameof(Strings.HelpSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.SolutionsOnly), getString(nameof(Strings.HelpSolutionsOnlySummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.ExcludePaths), getString(nameof(Strings.HelpExcludePathsSummary)), getString(nameof(Strings.HelpExcludePathsDescription)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages), getString(nameof(Strings.HelpPackagesSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages) + "." + nameof(NuGetPackageVersionPackageOptions.Name), getString(nameof(Strings.HelpNameSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages) + "." + nameof(NuGetPackageVersionPackageOptions.WarnExact), getString(nameof(Strings.HelpWarnExactSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages) + "." + nameof(NuGetPackageVersionPackageOptions.WarnOlder), getString(nameof(Strings.HelpWarnOlderSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages) + "." + nameof(NuGetPackageVersionPackageOptions.CriticalExact), getString(nameof(Strings.HelpCriticalExactSummary)))
                .AddConfiguration(nameof(NuGetPackageVersionOptions.Packages) + "." + nameof(NuGetPackageVersionPackageOptions.CriticalOlder), getString(nameof(Strings.HelpCriticalOlderSummary)))
                .BuildHtml();
        }
    }
}
