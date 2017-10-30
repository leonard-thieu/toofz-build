using System.Management.Automation;

namespace toofz.Build
{
    [Cmdlet(VerbsCommon.Get, "Project")]
    [OutputType(typeof(Project))]
    public sealed class GetProjectCmdlet : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        public string Path
        {
            get => path;
            set
            {
                path = System.IO.Path.IsPathRooted(value) ?
                    value :
                    System.IO.Path.Combine(CurrentProviderLocation("FileSystem").ProviderPath, value);
            }
        }
        private string path;

        protected override void ProcessRecord()
        {
            WriteObject(new Project(Path));
        }
    }
}
