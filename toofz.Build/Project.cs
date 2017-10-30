using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    internal sealed class Project
    {
        public Project(string filePath)
        {
            path = Path.GetDirectoryName(filePath);
            name = Path.GetFileNameWithoutExtension(filePath);

            project = XDocument.Load(filePath);
            var targetFramework = (from g in project.Root.Elements("PropertyGroup")
                                   from t in g.Elements("TargetFramework")
                                   select t.Value)
                                   .LastOrDefault();
            if (targetFramework == null)
            {
                throw new Exception();
            }

            TargetFramework = targetFramework;
            IsNetFramework = targetFramework.StartsWith("v");

            packagesPath = Path.Combine(path, "..", "packages");
            packages = new Lazy<IEnumerable<Package>>(ReadPackages);
        }

        private readonly string path;
        private readonly string name;
        private readonly XDocument project;
        private readonly string packagesPath;

        public string TargetFramework { get; }
        public bool IsNetFramework { get; }

        public IEnumerable<Package> Packages
        {
            get => packages.Value;
        }
        private readonly Lazy<IEnumerable<Package>> packages;

        public string GetPackageDirectory(string packageName)
        {
            var package = Packages.First(p => p.Name == packageName);

            return GetPackageDirectory(package);
        }

        public string GetPackageDirectory(Package package)
        {
            return IsNetFramework ?
                Path.Combine(packagesPath, $"{package.Name}.{package.Version}") :
                Path.Combine(packagesPath, package.Name.ToLower(), package.Version);
        }

        private IEnumerable<Package> ReadPackages()
        {
            if (IsNetFramework)
            {
                var packagesPath = Path.Combine(path, "packages.config");

                var doc = XDocument.Load(packagesPath);
                return (from p in doc.Root.Elements("package")
                        select new Package(p.Attribute("id").Value, p.Attribute("version").Value))
                        .ToList();
            }
            else
            {
                return (from p in project.Root.Elements("ItemGroup")
                        from r in p.Elements("PackageReference")
                        select new Package(p.Attribute("Include").Value, p.Attribute("Version").Value))
                        .ToList();
            }
        }

        public string GetOutPath(string configuration)
        {
            return IsNetFramework ?
                Path.Combine(path, "bin", configuration, $"{name}.dll") :
                Path.Combine(path, "bin", configuration, TargetFramework, $"{name}.dll");
        }
    }
}
