using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    public sealed class FrameworkProject : ProjectBase
    {
        public FrameworkProject(XDocument project) : base(project)
        {
            var targetFramework = (from pg in Project.Root.Elements()
                                   where pg.Name.LocalName == "PropertyGroup"
                                   from tfv in pg.Elements()
                                   where tfv.Name.LocalName == "TargetFrameworkVersion"
                                   select tfv.Value)
                                   .LastOrDefault();
            TargetFramework = targetFramework ?? throw new InvalidDataException("Unable to determine target framework for project.");

            packagesDir = Path.Combine(ProjectDir, "..", "packages");
        }

        private readonly string packagesDir;

        public override string TargetFramework { get; }

        public override string GetPackageDirectory(Package package)
        {
            return Path.Combine(packagesDir, $"{package.Name}.{package.Version}");
        }

        protected override IEnumerable<Package> ReadPackages()
        {
            var packagesPath = Path.Combine(ProjectDir, "packages.config");

            var doc = XDocument.Load(packagesPath);
            return (from p in doc.Root.Elements("package")
                    select new Package(p.Attribute("id").Value, p.Attribute("version").Value))
                    .ToList();
        }

        public override string GetOutPath(string configuration)
        {
            return Path.Combine(ProjectDir, "bin", configuration, $"{Name}.dll");
        }
    }
}
