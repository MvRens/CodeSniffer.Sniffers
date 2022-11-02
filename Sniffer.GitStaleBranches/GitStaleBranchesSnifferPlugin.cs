using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CodeSniffer.Core.Plugin;
using CodeSniffer.Core.Sniffer;
using JetBrains.Annotations;
using Serilog;

namespace Sniffer.GitStaleBranches
{
    [CsPlugin("ab11fe90-3a0d-462d-85b8-25db4eca3160", "Git stale branches")]
    [UsedImplicitly]
    public class GitStaleBranchesSnifferPlugin : ICsSnifferPlugin, ICsPluginHelp
    {
        public JsonObject? DefaultOptions => JsonSerializer.SerializeToNode(GitStaleBranchesOptions.Default()) as JsonObject;


        public ICsSniffer Create(ILogger logger, JsonObject options)
        {
            var gitStaleBranchesOptions = options.Deserialize<GitStaleBranchesOptions>();
            CsOptionMissingException.ThrowIfNull(gitStaleBranchesOptions);

            return new GitStaleBranchesSniffer(logger, gitStaleBranchesOptions);
        }


        public string? GetOptionsHelpHtml(IReadOnlyList<CultureInfo> cultures)
        {
            var getString = Strings.ResourceManager.CreateGetString(cultures);

            return CsPluginHelpBuilder.Create()
                .SetSummary(getString(nameof(Strings.HelpSummary)))
                .AddConfiguration(nameof(GitStaleBranchesOptions.MergeBranches), getString(nameof(Strings.HelpMergeBranches)))
                .BuildHtml();
        }
    }
}