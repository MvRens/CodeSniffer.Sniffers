using CodeSniffer.Core.Sniffer;
using LibGit2Sharp;
using Serilog;

namespace Sniffer.GitStaleBranches
{
    public class GitStaleBranchesSniffer : ICsSniffer
    {
        private readonly ILogger logger;
        private readonly GitStaleBranchesOptions options;


        public GitStaleBranchesSniffer(ILogger logger, GitStaleBranchesOptions options)
        {
            this.logger = logger;
            this.options = options;
        }


        public ValueTask<ICsReport?> Execute(string path, ICsScanContext context, CancellationToken cancellationToken)
        {
            if (options.MergeBranches == null || !options.MergeBranches.Contains(context.BranchName))
            {
                logger.Debug("Branch {branchName} is not in MergeBranches, skipping", context.BranchName);
                return ValueTask.FromResult<ICsReport?>(null);
            }

            if (!Repository.IsValid(path))
            {
                logger.Error(Strings.ResultNoWorkingCopy, path);
                return ValueTask.FromResult<ICsReport?>(null);
            }

            var builder = CsReportBuilder.Create();
            var repository = new Repository(path);

            foreach (var branch in repository.Branches)
            {
                //branch.Tip.
            }

            return ValueTask.FromResult<ICsReport?>(builder.Build());
        }
    }
}
