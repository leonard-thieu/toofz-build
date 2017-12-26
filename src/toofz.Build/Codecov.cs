using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build
{
    /// <summary>
    /// Executes the Codecov tool as a task.
    /// </summary>
    public sealed class Codecov : ToolTask
    {
        /// <summary>
        /// Path to coverage report.
        /// </summary>
        [Required]
        public ITaskItem File { get; set; }

        protected override string ToolName => "codecov.exe";

        protected override string GenerateFullPathToTool()
        {
            var path = ToolPath;
            var exe = Path.GetFileName(ToolExe);

            return Path.GetFullPath(Path.Combine(path, exe));
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendSwitch("--required");

            builder.AppendSwitchIfNotNull("--file=", File);

            return builder.ToString();
        }

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (string.IsNullOrWhiteSpace(singleLine)) { return; }
            if (IsLogoLine(singleLine)) { return; }

            const string LogLevelName = "logLevel";
            const string MessageName = "message";
            var LogMessagePattern = $@"^\d{{4}}-\d{{2}}-\d{{2}} \d{{2}}:\d{{2}}:\d{{2}} \[(?<{LogLevelName}>\w+)\] (?<{MessageName}>.*)";

            var match = Regex.Match(singleLine, LogMessagePattern);
            if (match.Success)
            {
                var logLevel = match.Groups[LogLevelName].Value;
                switch (logLevel)
                {
                    case "Information": messageImportance = MessageImportance.Low; break;
                    case "Warning": messageImportance = MessageImportance.Normal; break;
                    case "Fatal": messageImportance = MessageImportance.High; break;
                }

                singleLine = match.Groups[MessageName].Value;
            }
            else if (singleLine.StartsWith("   at "))
            {
                messageImportance = MessageImportance.High;
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);

            bool IsLogoLine(string line)
            {
                switch (line)
                {
                    case @"              _____          _":
                    case @"             / ____|        | |":
                    case @"            | |     ___   __| | ___  ___ _____   __":
                    case @"            | |    / _ \ / _  |/ _ \/ __/ _ \ \ / /":
                    case @"            | |___| (_) | (_| |  __/ (_| (_) \ V /":
                    case @"             \_____\___/ \____|\___|\___\___/ \_/":
                    case @"                                         exe-1.0.3":
                        return true;
                }

                return false;
            }
        }
    }
}
