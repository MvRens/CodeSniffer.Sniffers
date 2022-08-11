using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

        private readonly IReadOnlyList<MatcherFunc> excludeFoldersMatchers;
        private readonly IReadOnlyList<MatcherFunc> criticalMatchers;
        private readonly IReadOnlyList<MatcherFunc> warningMatchers;


        public DotNetVersionSniffer(ILogger logger, DotNetVersionOptions options)
        {
            this.logger = logger;
            this.options = options;

            excludeFoldersMatchers = CreateMatchers(options.ExcludeFolders);
            criticalMatchers = CreateMatchers(options.Critical);
            warningMatchers = CreateMatchers(options.Warn);
        }


        public async ValueTask<ICsReport?> Execute(string path, CancellationToken cancellationToken)
        {
            var builder = CsReportBuilder.Create();

            foreach (var projectFile in GetProjectFiles(path))
            {
                var projectName = Path.GetRelativePath(path, projectFile);

                if (Path.DirectorySeparatorChar != '\\')
                    projectName = projectName.Replace(Path.DirectorySeparatorChar, '\\');

                if (Matches(projectName, excludeFoldersMatchers))
                {
                    logger.Debug("Skipping project file {filename} because it matches one of the exclude folders", projectName);
                    continue;
                }

                var asset = builder.AddAsset(projectName, projectName);

                logger.Debug("Scanning project file {filename}", projectFile);
                try
                {
                    var result = CsReportResult.Critical;
                    var targetFrameworks = new StringBuilder();
                    asset.SetResult(CsReportResult.Critical, Strings.ResultNoTargetFrameworkVersion);

                    foreach (var targetFramework in await GetTargetFrameworks(projectFile, cancellationToken))
                    {
                        if (targetFrameworks.Length > 0)
                            targetFrameworks.Append(", ");

                        targetFrameworks.Append(targetFramework);

                        if (Matches(targetFramework, criticalMatchers))
                        {
                            // Only critical if all frameworks are critical
                            if (result == CsReportResult.Critical)
                                asset.SetResult(CsReportResult.Critical, string.Format(Strings.ResultCritical, targetFramework));
                        }
                        else if (Matches(targetFramework, warningMatchers))
                        {
                            // Only warning if all frameworks are either warning or critical
                            // ReSharper disable once InvertIf
                            if (result >= CsReportResult.Warning)
                            {
                                asset.SetResult(CsReportResult.Warning, string.Format(Strings.ResultWarning, targetFramework));
                                result = CsReportResult.Warning;
                            }
                        }
                        else
                        {
                            // If any framework is valid, consider the check a success
                            asset.SetResult(CsReportResult.Success, "");
                            result = CsReportResult.Success;
                        }
                    }

                    asset.SetProperty("Target framework", targetFrameworks.ToString());
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
                {
                    if (File.Exists(project.AbsolutePath))
                        yield return project.AbsolutePath;
                }
            }
        }


        private IEnumerable<string> GetAllProjectFiles(string path)
        {
            // I only use C#, so if you need support for other languages... it's open-source!
            logger.Debug("Scanning path {path} for C# projects", path);
            return Directory.GetFiles(path, @"*.csproj", SearchOption.AllDirectories);
        }


        private async ValueTask<IEnumerable<string>> GetTargetFrameworks(string projectFile, CancellationToken cancellationToken)
        {
            XElement project;
            await using (var stream = new FileStream(projectFile, FileMode.Open, FileAccess.Read))
            {
                project = await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken);
            }

            var versions = FindFirstNodeValue(project, "Project", "PropertyGroup", "TargetFrameworks");
            if (versions != null)
            {
                var versionValues = versions.Split(';').Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
                if (versionValues.Count > 0)
                    return versionValues;
            }

            var version =
                FindFirstNodeValue(project, "Project", "PropertyGroup", "TargetFramework") ??
                FindFirstNodeValue(project, "Project", "PropertyGroup", "TargetFrameworkVersion");

            if (!string.IsNullOrWhiteSpace(version))
                return Enumerable.Repeat(version, 1);
            
            logger.Warning(
                "Could not find TargetFramework or TargetFrameworkVersion element in Project file {filename}, skipping",
                projectFile);

            return Enumerable.Empty<string>();
        }


        private static string? FindFirstNodeValue(XElement document, string documentElement, params string[] path)
        {
            if (path.Length == 0)
                return null;

            if (document.Name.LocalName != documentElement)
                return null;

            // XPath insists on using namespaces, this is easier and good enough
            return path
                .Aggregate(
                    new [] { document } as IEnumerable<XElement>, 
                    (current, part) => current.SelectMany(n => n.Elements().Where(e => e.Name.LocalName == part)))
                .Where(e => !string.IsNullOrWhiteSpace(e.Value))
                .Select(e => e.Value)
                .FirstOrDefault();
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
