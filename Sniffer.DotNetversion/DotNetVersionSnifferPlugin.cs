using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using JetBrains.Annotations;
using Serilog;

namespace Sniffer.DotNetversion
{
    [CsPlugin("fe7d8b97-efd3-44c2-81b6-5f86c0c5f684")]
    [UsedImplicitly]
    public class DotNetVersionSnifferPlugin : ICsSnifferPlugin, ICsPluginHelp
    {
        public string Name => ".NET target framework version";
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(DotNetVersionOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var dotnetVersionOptions = options.Deserialize<DotNetVersionOptions>();
            CsOptionMissingException.ThrowIfNull(dotnetVersionOptions);

            return new DotNetVersionSniffer(logger, dotnetVersionOptions);
        }


        public string? GetOptionsHelpHtml(IReadOnlyList<CultureInfo> cultures)
        {
            var getString = Strings.ResourceManager.CreateGetString(cultures);

            return CsPluginHelpBuilder.Create()
                .SetSummary(getString(nameof(Strings.HelpSummary)))
                .AddConfiguration(nameof(DotNetVersionOptions.SolutionsOnly), getString(nameof(Strings.HelpSolutionsOnlySummary)))
                .AddConfiguration(nameof(DotNetVersionOptions.ExcludeFolders), getString(nameof(Strings.HelpExcludeFoldersSummary)), getString(nameof(Strings.HelpExcludeFoldersDescription)))
                .AddConfiguration(nameof(DotNetVersionOptions.Warn), getString(nameof(Strings.HelpWarnSummary)), getString(nameof(Strings.HelpWarnDescription)))
                .AddConfiguration(nameof(DotNetVersionOptions.Critical), getString(nameof(Strings.HelpCriticalSummary)))
                .BuildHtml();
        }
    }
}