namespace toofz.Build
{
    internal sealed class Package
    {
        public Package(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }
        public string Version { get; }
    }
}