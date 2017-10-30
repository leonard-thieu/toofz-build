using System.Management.Automation;

namespace toofz.Build
{
    [Cmdlet(VerbsCommon.Get, "Project")]
    public sealed class GetProjectCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(new Project(Path));
        }
    }
}
