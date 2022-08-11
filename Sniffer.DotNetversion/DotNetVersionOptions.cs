using System.Text.Json.Serialization;

namespace Sniffer.DotNetversion
{
    [JsonSerializable(typeof(DotNetVersionOptions))]
    public class DotNetVersionOptions
    {
        /// <summary>
        /// If True, only projects in Solution files will be considered. Otherwise all project files
        /// will be checked. Defaults to False.
        /// </summary>
        public bool SolutionsOnly { get; set; } = false;

        /// <summary>
        /// The folders to exclude, relative to the root of the repository.
        /// </summary>
        /// <remarks>
        /// If a folder starts and ends with a forward slash (/), it is considered a regular expression.
        /// If it contains either an asterisk (*) or question mark (?), these are considered wildcards for
        /// respectively matching 0 or more characters (*) or exactly one character (?).
        /// Otherwise, the folder must match exactly (case-insensitive).
        /// <br /><br />
        /// The path separator is a backwards slash regardless of the operating system CodeSniffer runs on,
        /// to prevent conflicts.
        /// </remarks> 
        public string[]? ExcludeFolders { get; set; }

        /// <summary>
        /// The names of the .NET versions, as specified in the project files, which should cause a warning.
        /// </summary>
        /// <remarks>
        /// If a filter starts and ends with a forward slash (/), it is considered a regular expression.
        /// If it contains either an asterisk (*) or question mark (?), these are considered wildcards for
        /// respectively matching 0 or more characters (*) or exactly one character (?).
        /// Otherwise, the version must match exactly (case-insensitive).
        /// </remarks>
        public string[]? Warn { get; set; }

        /// <summary>
        /// The names of the .NET versions, as specified in the project files, which should be considered critical.
        /// </summary>
        /// <remarks>
        /// If a filter starts and ends with a forward slash (/), it is considered a regular expression.
        /// If it contains either an asterisk (*) or question mark (?), these are considered wildcards for
        /// respectively matching 0 or more characters (*) or exactly one character (?).
        /// Otherwise, the version must match exactly (case-insensitive).
        /// </remarks> 
        public string[]? Critical { get; set; }


        public static DotNetVersionOptions Default()
        {
            return new DotNetVersionOptions
            {
                Warn = new[] { "netcoreapp*", "net5.0" },
                Critical = new[] { "/v4\\.7(?!\\.2)|v4(?!\\.7)|v[1-3]/", "netcoreapp2.1" }
            };
        }
    }
}
