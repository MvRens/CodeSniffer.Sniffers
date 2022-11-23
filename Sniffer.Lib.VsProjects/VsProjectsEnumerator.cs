using System.Diagnostics.CodeAnalysis;
using CodeSniffer.Core.Sniffer;
using Microsoft.Build.Construction;
using Serilog;
using Sniffer.Lib.Matchers.cs;

namespace Sniffer.Lib.VsProjects
{
    public class VsProjectsEnumerator
    {
        private readonly ILogger logger;
        private readonly CsReportBuilder builder;
        private readonly IMatcher excludePathsMatcher;


        public VsProjectsEnumerator(ILogger logger, CsReportBuilder builder, IMatcher excludePathsMatcher)
        {
            this.logger = logger;
            this.builder = builder;
            this.excludePathsMatcher = excludePathsMatcher;
        }


        public IEnumerable<VsProject> GetProjects(string path, bool solutionsOnly)
        {
            return solutionsOnly
                ? GetSolutionProjects(path)
                : GetAllProjectFiles(path);
        }



        private IEnumerable<VsProject> GetSolutionProjects(string path)
        {
            logger.Debug("Scanning path {path} for Visual Studio solutions", path);

            foreach (var solutionFile in Directory.GetFiles(path, @"*.sln", SearchOption.AllDirectories))
            {
                var solutionName = Path.GetRelativePath(path, solutionFile);
                if (excludePathsMatcher.Matches(solutionName))
                {
                    builder
                        .AddAsset(solutionName, solutionName)
                        .SetResult(CsReportResult.Skipped, Strings.ResultSkippedExcludePaths);
                    continue;
                }

                logger.Debug("Parsing solution file {filename}", solutionFile);

                var solution = SolutionFile.Parse(solutionFile);
                foreach (var project in solution.ProjectsInOrder.Where(p =>
                             p.ProjectType != SolutionProjectType.SolutionFolder))
                {
                    if (ValidVsProject(path, project.AbsolutePath, out var vsProject))
                        yield return vsProject.Value;
                }
            }
        }


        private IEnumerable<VsProject> GetAllProjectFiles(string path)
        {
            // I only use C#, so if you need support for other languages... it's open-source!
            logger.Debug("Scanning path {path} for C# projects", path);

            return Directory.GetFiles(path, @"*.csproj", SearchOption.AllDirectories)
                .Select(f => ValidVsProject(path, f, out var vsProject) ? vsProject : null)
                .Where(p => p != null)
                .Cast<VsProject>();
        }


        private bool ValidVsProject(string basePath, string absolutePath, [NotNullWhen(true)] out VsProject? project)
        {
            if (!File.Exists(absolutePath))
            {
                project = null;
                return false;
            }

            var projectName = Path.GetRelativePath(basePath, absolutePath);

            if (Path.DirectorySeparatorChar != '\\')
                projectName = projectName.Replace(Path.DirectorySeparatorChar, '\\');

            var asset = builder.AddAsset(projectName, projectName);

            if (excludePathsMatcher.Matches(projectName))
            {
                asset.SetResult(CsReportResult.Skipped, Strings.ResultSkippedExcludePaths);
                project = null;
                return false;
            }

            project = new VsProject(projectName, absolutePath, asset);
            return true;
        }
    }
}