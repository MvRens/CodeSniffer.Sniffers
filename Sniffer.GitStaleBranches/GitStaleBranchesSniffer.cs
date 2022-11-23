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
                logger.Error("The checkout at {path} is not a valid Git repository", path);
                return ValueTask.FromResult<ICsReport?>(null);
            }

            var builder = CsReportBuilder.Create();
            var repository = new Repository(path);


            // Look up MergeBranches in the repository
            var referenceBranches = options.MergeBranches
                .Select(mb =>
                {
                    var branch = repository.Branches.FirstOrDefault(b => b.IsRemote && GetBranchName(b) == mb);
                    if (branch == null)
                        return null;

                    var refs = repository.Refs.Where(r => r.CanonicalName == branch.CanonicalName).ToArray();
                    if (refs.Length == 0)
                        return null;

                    return new ReferenceBranch(branch, refs);
                })
                .Where(rb => rb != null)
                .Cast<ReferenceBranch>()
                .ToArray();

            if (referenceBranches.Length == 0)
                // Should never occur because the working copy is on one of those branches
                return ValueTask.FromResult<ICsReport?>(null);


            foreach (var branch in repository.Branches.Where(b => b.IsRemote && GetBranchName(b) != "HEAD"))
            {
                // Skip the MergeBranches
                if (referenceBranches.Any(rb => rb.Branch == branch))
                    continue;

                var branchName = GetBranchName(branch);
                var branchAsset = builder.AddAsset(branchName, branchName);
                if (!CheckMerged(branchAsset, repository, referenceBranches, branch))
                    CheckStale(branchAsset, branch, options);
            }

            return ValueTask.FromResult<ICsReport?>(builder.Build());
        }


        private static bool CheckMerged(CsReportBuilder.Asset branchAsset, IRepository repository, IEnumerable<ReferenceBranch> referenceBranches, Branch branch)
        {
            var mergedInto = referenceBranches.FirstOrDefault(rb => repository.Refs.ReachableFrom(rb.Refs, new[] { branch.Tip }).Any());
            if (mergedInto == null)
                return false;


            var mergeCommits = mergedInto.Branch.Commits.Where(c => c.Parents.Contains(branch.Tip));
            var mergeCommit = mergeCommits.MinBy(c => c.Committer.When);

            if (mergeCommit == null)
                return false;


            branchAsset
                .SetResultIfHigher(CsReportResult.Warning, string.Format(Strings.ResultMerged, GetBranchName(mergedInto.Branch)))
                .AddToOutput(FormatCommitOutput(Strings.ResultMergedOutput, mergeCommit));
            return true;
        }


        private static void CheckStale(CsReportBuilder.Asset branchAsset, Branch branch, GitStaleBranchesOptions options)
        {
            var staleAfterDays = options.StaleAfterDays.GetValueOrDefault();
            if (staleAfterDays <= 0)
                return;

            var daysAgo = (int)Math.Ceiling((DateTimeOffset.UtcNow - branch.Tip.Committer.When.ToUniversalTime()).TotalDays);
            if (daysAgo < staleAfterDays)
                return;

            branchAsset
                .SetResultIfHigher(CsReportResult.Warning, string.Format(Strings.ResultStale, daysAgo))
                .AddToOutput(FormatCommitOutput(Strings.ResultStaleOutput, branch.Tip));
        }


        private static string GetBranchName(Branch branch)
        {
            return branch.FriendlyName.StartsWith($"{branch.RemoteName}/")
                ? branch.FriendlyName[(branch.RemoteName.Length + 1)..]
                : branch.FriendlyName;

        }

        private static string FormatCommitOutput(string format, Commit commit)
        {
            return string.Format(format,
                commit.Committer.When.ToString("D"),
                commit.Committer.Name,
                commit.Committer.Email,
                commit.Sha);
        }


        private class ReferenceBranch
        {
            public Branch Branch { get; }
            public Reference[] Refs { get; }


            public ReferenceBranch(Branch branch, Reference[] refs)
            {
                Branch = branch;
                Refs = refs;
            }
        }
    }
}
