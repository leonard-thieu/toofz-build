using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    public sealed class CoreProject : ProjectBase
    {
        public CoreProject(XDocument project) : base(project)
        {
            TargetFramework = GetProperties("TargetFramework").LastOrDefault()?.Value ??
                throw new InvalidDataException("Unable to determine target framework for project.");

            packagesDir = Path.Combine(ProjectDir, "..", "packages");
        }

        private readonly string packagesDir;

        public override string TargetFramework { get; }
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }

        public override string GetPackageDirectory(Package package)
        {
            return Path.Combine(packagesDir, package.Name.ToLower(), package.Version);
        }

        protected override IEnumerable<Package> ReadPackages()
        {
            return (from ig in Project.Root.Elements("ItemGroup")
                    from pr in ig.Elements("PackageReference")
                    select new Package(pr.Attribute("Include").Value, pr.Attribute("Version").Value))
                    .ToList();
        }

        public override string GetOutPath(string configuration)
        {
            return Path.Combine(ProjectDir, "bin", configuration, TargetFramework, $"{Name}.dll");
        }

        private void ReadNuspec()
        {
            PackageId = GetProperties("PackageId").LastOrDefault()?.Value ?? Name;
            PackageVersion = GetProperties("Version").LastOrDefault()?.Value ?? "1.0.0";
        }
    }
}
