using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using Serilog;

namespace Sniffer.DotNetversion
{
    [CsPlugin("dotnet.version")]
    public class DotNetVersionSnifferPlugin : ICsSnifferPlugin
    {
        public string Name => ".NET target framework version";
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(DotNetVersionOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var dotnetVersionOptions = options.Deserialize<DotNetVersionOptions>();
            CsOptionMissingException.ThrowIfNull(dotnetVersionOptions);

            return new DotNetVersionSniffer(logger, dotnetVersionOptions);
        }
    }
}