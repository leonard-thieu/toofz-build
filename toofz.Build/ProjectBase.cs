﻿using System;
using System.Collections.Generic;
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
            if (sdkAttr != null && sdkAttr.Value == "Microsoft.NET.Sdk")
            {
                return new CoreProject(project);
            }
            return new FrameworkProject(project);
        }

        protected ProjectBase(XDocument project)
        {
            Project = project;
            packages = new Lazy<IEnumerable<Package>>(ReadPackages);
        }

        private readonly Lazy<IEnumerable<Package>> packages;

        protected XDocument Project { get; }

        public string ProjectDir { get; }
        public string Name { get; }
        public abstract string TargetFramework { get; }

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
                    from tf in pg.ElementsByLocalName(localName)
                    select tf)
                    .ToList();
        }
    }
}
