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
            var targetFramework = (from pg in Project.Root.Elements()
                                   where pg.Name.LocalName == "PropertyGroup"
                                   from tf in pg.Elements()
                                   where tf.Name.LocalName == "TargetFramework"
                                   select tf.Value)
                                   .LastOrDefault();
            TargetFramework = targetFramework ?? throw new InvalidDataException("Unable to determine target framework for project.");

            packagesDir = Path.Combine(ProjectDir, "..", "packages");
        }

        private readonly string packagesDir;

        public override string TargetFramework { get; }

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
    }
}
