using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using JetBrains.Annotations;
using Serilog;

namespace Sniffer.VueI18n
{
    [CsPlugin("b476b950-79db-4a1c-927c-49f16bc316b5", "VueI18n")]
    [UsedImplicitly]
    public class VueI18NSnifferPlugin : ICsSnifferPlugin, ICsPluginHelp
    {
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(VueI18NOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var vuei18NOptions = options.Deserialize<VueI18NOptions>();
            CsOptionMissingException.ThrowIfNull(vuei18NOptions);

            return new VueI18NSniffer(logger, vuei18NOptions);
        }


        public string? GetOptionsHelpHtml(IReadOnlyList<CultureInfo> cultures)
        {
            var getString = Strings.ResourceManager.CreateGetString(cultures);

            return CsPluginHelpBuilder.Create()
                .SetSummary(getString(nameof(Strings.HelpSummary)))
                //.AddConfiguration(nameof(VueI18nOptions.), getString(nameof(Strings.)))
                .BuildHtml();
        }
    }
}
