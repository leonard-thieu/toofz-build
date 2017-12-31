using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build
{
    /// <summary>
    /// Executes the NuGet push command as a task.
    /// </summary>
    public sealed class NuGetPush : ToolTask
    {
        /// <summary>
        /// Path to the package.
        /// </summary>
        [Required]
        public ITaskItem Package { get; set; }
        /// <summary>
        /// Specifies the server URL. If not specified, nuget.org is used unless 
        /// DefaultPushSource config value is set in the NuGet config file.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The API key for the server.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// Specifies the symbol server URL. If not specified, 
        /// nuget.smbsrc.net is used when pushing to nuget.org.
        /// </summary>
        public string SymbolSource { get; set; }
        /// <summary>
        /// The API key for the symbol server.
        /// </summary>
        public string SymbolApiKey { get; set; }

        /// <summary>
        /// The name of the NuGet tool.
        /// </summary>
        protected override string ToolName => "NuGet.exe";

        /// <summary>
        /// Generates the full path to the NuGet tool.
        /// </summary>
        /// <returns>
        /// The full path to the NuGet tool.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            var path = ToolPath;
            var exe = Path.GetFileName(ToolExe);

            return Path.GetFullPath(Path.Combine(path, exe));
        }

        /// <summary>
        /// Generates command line arguments for the NuGet tool.
        /// </summary>
        /// <returns>
        /// Command line arguments for the NuGet tool.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendSwitch("push");
            builder.AppendSwitchIfNotNull("-Verbosity ", "detailed");

            builder.AppendFileNameIfNotNull(Package);
            builder.AppendSwitchIfNotNull("-Source ", Source);
            builder.AppendSwitchIfNotNull("-ApiKey ", ApiKey);
            builder.AppendSwitchIfNotNull("-SymbolSource ", SymbolSource);
            builder.AppendSwitchIfNotNull("-SymbolApiKey ", SymbolApiKey);

            return builder.ToString();
        }
    }
}
