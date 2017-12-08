using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace toofz.Build
{
    public abstract class ProjectBase
    {
        public static ProjectBase Create(string filePath)
        {
            var project = XDocument.Load(filePath);
            var sdkAttr = project.Root.Attribute("Sdk");
            if (sdkAttr?.Value == "Microsoft.NET.Sdk")
            {
                return new CoreProject(project, filePath);
            }
            return new FrameworkProject(project, filePath);
        }

        protected ProjectBase(XDocument project, string filePath)
        {
            Project = project;
            ProjectDir = Path.GetDirectoryName(filePath);
            Name = Path.GetFileNameWithoutExtension(filePath);
            packages = new Lazy<IEnumerable<Package>>(ReadPackages);
            PackageId = GetProperties("PackageId").LastOrDefault()?.Value ?? Name;
            PackageVersion = GetProperties("PackageVersion").LastOrDefault()?.Value ?? "1.0.0";
        }

        private readonly Lazy<IEnumerable<Package>> packages;

        protected XDocument Project { get; }

        public string ProjectDir { get; }
        public string Name { get; }
        public abstract string TargetFramework { get; }
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }

        #region Packages

        public IEnumerable<Package> Packages { get; }

        public string GetPackageDirectory(string packageName)
        {
            var package = Packages.First(p => p.Name == packageName);

            return GetPackageDirectory(package);
        }

        public abstract string GetPackageDirectory(Package package);

        protected abstract IEnumerable<Package> ReadPackages();

        #endregion

        public abstract string GetOutPath(string configuration);

        protected IEnumerable<XElement> GetProperties(string localName)
        {
            return (from pg in Project.Root.ElementsByLocalName("PropertyGroup")
                    from p in pg.ElementsByLocalName(localName)
                    select p)
                    .ToList();
        }
    }
}
