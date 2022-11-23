using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using JetBrains.Annotations;
using Serilog;

namespace Sniffer.Grep
{
    [CsPlugin("b476b950-79db-4a1c-927c-49f16bc316b5", "Grep")]
    [UsedImplicitly]
    public class GrepSnifferPlugin : ICsSnifferPlugin, ICsPluginHelp
    {
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(GrepOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var grepOptions = options.Deserialize<GrepOptions>();
            CsOptionMissingException.ThrowIfNull(grepOptions);

            return new GrepSniffer(logger, grepOptions);
        }


        public string? GetOptionsHelpHtml(IReadOnlyList<CultureInfo> cultures)
        {
            var getString = Strings.ResourceManager.CreateGetString(cultures);

            return CsPluginHelpBuilder.Create()
                .SetSummary(getString(nameof(Strings.HelpSummary)))
                //.AddConfiguration(nameof(GrepOptions.), getString(nameof(Strings.)))
                .BuildHtml();
        }
    }
}