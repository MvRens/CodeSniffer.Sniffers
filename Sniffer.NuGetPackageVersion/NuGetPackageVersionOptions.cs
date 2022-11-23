using System.Text.Json.Serialization;

namespace Sniffer.NuGetPackageVersion
{
    [JsonSerializable(typeof(NuGetPackageVersionOptions))]
    public class NuGetPackageVersionOptions
    {
        public bool SolutionsOnly { get; set; } = false;
        public string[]? ExcludePaths { get; set; }
        public NuGetPackageVersionPackageOptions[]? Packages { get; set; }


        public static NuGetPackageVersionOptions Default()
        {
            return new NuGetPackageVersionOptions
            {
                Packages = new []
                {
                    new NuGetPackageVersionPackageOptions
                    {
                        Name = "Example.Package",
                        WarnExact = "4.20",
                        CriticalOlder = "4.0"
                    }
                }
            };
        }
    }


    [JsonSerializable(typeof(NuGetPackageVersionOptions))]
    public class NuGetPackageVersionPackageOptions
    {
        public string? Name { get; set; }
        public string? WarnExact { get; set; }
        public string? WarnOlder { get; set; }
        public string? CriticalExact { get; set; }
        public string? CriticalOlder { get; set; }
    }
}
