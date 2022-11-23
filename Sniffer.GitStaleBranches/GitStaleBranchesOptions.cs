using System.Text.Json.Serialization;

namespace Sniffer.GitStaleBranches
{
    [JsonSerializable(typeof(GitStaleBranchesOptions))]
    public class GitStaleBranchesOptions
    {
        public string[]? MergeBranches { get; set; }
        public int? StaleAfterDays { get; set; }


        public static GitStaleBranchesOptions Default()
        {
            return new GitStaleBranchesOptions
            {
                MergeBranches = new [] { "master", "main", "develop" },
                StaleAfterDays = 90
            };
        }
    }
}
