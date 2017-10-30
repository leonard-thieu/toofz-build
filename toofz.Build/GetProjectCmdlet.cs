using System.IO;
using System.Management.Automation;

namespace toofz.Build
{
    [Cmdlet(VerbsCommon.Get, "Project")]
    [OutputType(typeof(ProjectBase))]
    public sealed class GetProjectCmdlet : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        public string ProjectPath
        {
            get => path;
            set
            {
                path = Path.IsPathRooted(value) ?
                    value :
                    Path.Combine(CurrentProviderLocation("FileSystem").ProviderPath, value);
            }
        }
        private string path;

        protected override void ProcessRecord()
        {
            WriteObject(ProjectBase.Create(ProjectPath));
        }
    }
}
