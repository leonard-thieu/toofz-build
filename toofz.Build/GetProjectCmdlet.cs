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
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new Project(Path));
        }
    }
}
