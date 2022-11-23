using CodeSniffer.Core.Sniffer;

namespace Sniffer.Lib.VsProjects
{
    public readonly struct VsProject
    {
        public string ProjectName { get; }
        public string Filename { get; }

        public CsReportBuilder.Asset Asset { get; }


        public VsProject(string projectName, string filename, CsReportBuilder.Asset asset)
        {
            ProjectName = projectName;
            Filename = filename;
            Asset = asset;
        }
    }
}
