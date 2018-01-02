using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build
{
    /// <summary>
    /// Pushes an artifact to AppVeyor.
    /// </summary>
    public sealed class AppVeyorPushArtifact : ToolTask
    {
        /// <summary>
        /// Local path to artifact file or directory.
        /// </summary>
        [Required]
        public ITaskItem Path { get; set; }

        /// <summary>
        /// Artifact file name in the cloud storage.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Deployment name.
        /// </summary>
        public string DeploymentName { get; set; }

        /// <summary>
        /// Type. <see cref="ArtifactType.Auto"/> if not specified. 
        /// Possible values: <see cref="ArtifactType.Auto"/>, <see cref="ArtifactType.WebDeployPackage"/>, <see cref="ArtifactType.NuGetPackage"/>, 
        /// <see cref="ArtifactType.AzureCloudService"/>, <see cref="ArtifactType.AzureCloudServiceConfig"/>, <see cref="ArtifactType.SsdtPackage"/>, 
        /// <see cref="ArtifactType.Zip"/>, <see cref="ArtifactType.File"/>
        /// </summary>
        public string Type
        {
            get { return type.ToString(); }
            set { type = (ArtifactType)Enum.Parse(typeof(ArtifactType), value, ignoreCase: true); }
        }
        private ArtifactType type;

        /// <summary>
        /// Upload progress to display.
        /// </summary>
        public string Verbosity
        {
            get { return verbosity.ToString(); }
            set { verbosity = (UploadVerbosity)Enum.Parse(typeof(UploadVerbosity), value, ignoreCase: true); }
        }
        private UploadVerbosity verbosity;

        /// <summary>
        /// Importance with which to log text from in the standard out stream.
        /// </summary>
        protected override MessageImportance StandardOutputLoggingImportance => MessageImportance.High;

        /// <summary>
        /// Gets the name of the AppVeyor tool.
        /// </summary>
        protected override string ToolName => "appveyor.exe";

        /// <summary>
        /// Gets a value indicating to search for the AppVeyor tool in the system path.
        /// </summary>
        /// <returns>
        /// The value of <see cref="ToolTask.ToolExe"/> which indicates to search for the AppVeyor tool in the system path.
        /// </returns>
        protected override string GenerateFullPathToTool() => ToolExe;

        /// <summary>
        /// Generates command line arguments for the AppVeyor tool's PushArtifact command.
        /// </summary>
        /// <returns>
        /// Command line arguments for the AppVeyor tool's PushArtifact command.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendSwitch("PushArtifact");

            builder.AppendSwitchIfNotNull("-Path ", Path);
            builder.AppendSwitchIfNotNull("-FileName ", FileName);
            builder.AppendSwitchIfNotNull("-DeploymentName ", DeploymentName);
            builder.AppendSwitchIfNotNull("-Type ", Type);
            builder.AppendSwitchIfNotNull("-Verbosity ", Verbosity);

            return builder.ToString();
        }

        private enum ArtifactType
        {
            Auto,
            WebDeployPackage,
            NuGetPackage,
            AzureCloudService,
            AzureCloudServiceConfig,
            SsdtPackage,
            Zip,
            File,
        }

        private enum UploadVerbosity
        {
            Normal,
            Minimal,
            Quiet,
        }
    }
}
