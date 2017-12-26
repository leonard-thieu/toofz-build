using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build.Tasks
{
    public class NuGetPush : ToolTask
    {
        [Required]
        public ITaskItem Package { get; set; }
        public string Source { get; set; }
        public string ApiKey { get; set; }
        public string SymbolSource { get; set; }
        public string SymbolApiKey { get; set; }

        protected override string ToolName => "NuGet.exe";

        protected override string GenerateFullPathToTool()
        {
            var path = ToolPath;
            var exe = Path.GetFileName(ToolExe);

            return Path.GetFullPath(Path.Combine(path, exe));
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();
            builder.AppendSwitch("push");

            builder.AppendFileNameIfNotNull(Package);
            builder.AppendSwitchIfNotNull("-Source ", Source);
            builder.AppendSwitchIfNotNull("-ApiKey ", ApiKey);
            builder.AppendSwitchIfNotNull("-SymbolSource ", SymbolSource);
            builder.AppendSwitchIfNotNull("-SymbolApiKey ", SymbolApiKey);

            return builder.ToString();
        }
    }
}
