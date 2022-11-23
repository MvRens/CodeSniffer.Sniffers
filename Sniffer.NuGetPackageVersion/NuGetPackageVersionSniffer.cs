using CodeSniffer.Core.Sniffer;
using Serilog;
using Sniffer.Lib.Matchers.cs;
using Sniffer.Lib.VsProjects;

namespace Sniffer.NuGetPackageVersion
{
    public class NuGetPackageVersionSniffer : ICsSniffer
    {
        private readonly ILogger logger;
        private readonly NuGetPackageVersionOptions options;

        private readonly IMatcher excludePathsMatcher;


        public NuGetPackageVersionSniffer(ILogger logger, NuGetPackageVersionOptions options)
        {
            this.logger = logger;
            this.options = options;

            excludePathsMatcher = MatcherFactory.Create(options.ExcludePaths ?? Enumerable.Empty<string>());
        }


        public async ValueTask<ICsReport?> Execute(string path, ICsScanContext context, CancellationToken cancellationToken)
        {
            var builder = CsReportBuilder.Create();
            var projectsEnumerator = new VsProjectsEnumerator(logger, builder, excludePathsMatcher);

            foreach (var project in projectsEnumerator.GetProjects(path, options.SolutionsOnly))
            {
                // TODO check for NuGet package versions
            }

            return builder.Build();
        }
    }
}
