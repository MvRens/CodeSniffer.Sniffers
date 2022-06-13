using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using CodeSniffer.Core.Sniffer;
using Microsoft.Build.Construction;
using Serilog;

namespace Sniffer.DotNetversion
{
    public class DotNetVersionSniffer : ICsSniffer
    {
        private delegate bool MatcherFunc(string value);

        private readonly ILogger logger;
        private readonly DotNetVersionOptions options;

        private readonly IReadOnlyList<MatcherFunc> criticalMatchers;
        private readonly IReadOnlyList<MatcherFunc> warningMatchers;


        public DotNetVersionSniffer(ILogger logger, DotNetVersionOptions options)
        {
            this.logger = logger;
            this.options = options;

            criticalMatchers = CreateMatchers(options.Critical);
            warningMatchers = CreateMatchers(options.Warn);
        }


        public ICsReport Execute(string path)
        {
            var builder = CsReportBuilder.Create();

            foreach (var projectFile in GetProjectFiles(path))
            {
                var projectName = Path.GetRelativePath(path, projectFile);
                var asset = builder.AddAsset(projectName, projectName);

                logger.Debug("Scanning project file {filename}", projectFile);
                try
                {
                    if (!TryGetTargetFramework(projectFile, out var targetFramework))
                    {
                        asset.SetResult(CsReportResult.Critical, "Unable to determine target framework version");
                        continue;
                    }

                    asset.SetProperty("Target framework", targetFramework);

                    if (Matches(targetFramework, criticalMatchers))
                        asset.SetResultIfHigher(CsReportResult.Critical, "Target framework is in the critical list");
                    else if (Matches(targetFramework, warningMatchers))
                        asset.SetResultIfHigher(CsReportResult.Warning, "Target framework is in the warning list");
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error while parsing project file {filename}: {message}", projectFile, e.Message);
                }
            }

            return builder.Build();
        }


        private IEnumerable<string> GetProjectFiles(string path)
        {
            return options.SolutionsOnly
                ? GetSolutionProjectFiles(path)
                : GetAllProjectFiles(path);
        }



        private IEnumerable<string> GetSolutionProjectFiles(string path)
        {
            logger.Debug("Scanning path {path} for Visual Studio solutions", path);

            foreach (var solutionFile in Directory.GetFiles(path, @"*.sln", SearchOption.AllDirectories))
            {
                logger.Debug("Parsing solution file {filename}", solutionFile);

                var solution = SolutionFile.Parse(solutionFile);
                foreach (var project in solution.ProjectsInOrder.Where(p =>
                             p.ProjectType != SolutionProjectType.SolutionFolder))
                    yield return project.AbsolutePath;
            }
        }


        private IEnumerable<string> GetAllProjectFiles(string path)
        {
            // I only use C#, so if you need support for other languages... it's open-source!
            logger.Debug("Scanning path {path} for C# projects", path);
            return Directory.GetFiles(path, @"*.csproj", SearchOption.AllDirectories);
        }


        private bool TryGetTargetFramework(string projectFile, [NotNullWhen(true)] out string? version)
        {
            var project = new XPathDocument(projectFile);
            var navigator = project.CreateNavigator();

            var versionNode =
                navigator.SelectSingleNode("/Project/PropertyGroup/TargetFramework") ??
                navigator.SelectSingleNode("/Project/PropertyGroup/TargetFrameworkVersion");

            version = versionNode?.Value;
            if (!string.IsNullOrWhiteSpace(version))
                return true;

            logger.Warning(
                "Could not find TargetFramework or TargetFrameworkVersion element in Project file {filename}, skipping",
                projectFile);
            return false;
        }


        private static bool Matches(string value, IEnumerable<MatcherFunc> matchers)
        {
            return matchers.Any(m => m(value));
        }


        private static IReadOnlyList<MatcherFunc> CreateMatchers(IReadOnlyCollection<string>? versions)
        {
            if (versions == null || versions.Count == 0)
                return Array.Empty<MatcherFunc>();

            return versions
                .Select(v =>
                    v.StartsWith("/") && v.EndsWith("/")
                        ? CreateRegexMatcher(v[1..^1])
                        : v.Contains('*') || v.Contains('?')
                            ? CreateWildcardMatcher(v)
                            : CreateStaticMatcher(v))
                .Where(m => m != null)
                .Select(m => m!)
                .ToList();
        }


        private static MatcherFunc? CreateRegexMatcher(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return null;

            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return v => regex.IsMatch(v);
        }


        private static MatcherFunc? CreateWildcardMatcher(string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            return CreateRegexMatcher(regexPattern);
        }


        private static MatcherFunc? CreateStaticMatcher(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return null;

            return v => v.Equals(pattern, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
