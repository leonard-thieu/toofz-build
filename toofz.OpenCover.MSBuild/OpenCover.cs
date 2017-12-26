using Microsoft.Build.Utilities;

namespace toofz.OpenCover.MSBuild
{
    public class OpenCover : global::OpenCover.MSBuild.OpenCover
    {
        public bool OldStyle { get; set; }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            if (OldStyle) { builder.AppendSwitch("-oldstyle"); }

            return string.Join(" ", base.GenerateCommandLineCommands(), builder.ToString());
        }
    }
}
