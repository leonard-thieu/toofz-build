using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    public sealed class Project
    {
        public Project(string filePath)
        {
            path = Path.GetDirectoryName(filePath);
            name = Path.GetFileNameWithoutExtension(filePath);

            project = XDocument.Load(filePath);

            // Can't use Elements(XName name) because .NET framework projects have a namespace.
            var propertyGroups = (from g in project.Root.Elements()
                                  where g.Name.LocalName == "PropertyGroup"
                                  select g)
                                  .ToList();

            Version = (from v in propertyGroups.Elements()
                       where v.Name.LocalName == "Version"
                       select v.Value)
                       .LastOrDefault();

            // .NET Framework uses "TargetFrameworkVersion"
            // .NET Core/Standard uses "TargetFramework"
            var targetFramework = (from t in propertyGroups.Elements()
                                   where t.Name.LocalName == "TargetFrameworkVersion" || t.Name.LocalName == "TargetFramework"
                                   select t.Value)
                                   .LastOrDefault();
            TargetFramework = targetFramework ?? throw new InvalidDataException("Unable to determine target framework for project.");
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
        public string Version { get; }

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
                return (from ig in project.Root.Elements("ItemGroup")
                        from pr in ig.Elements("PackageReference")
                        select new Package(pr.Attribute("Include").Value, pr.Attribute("Version").Value))
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
