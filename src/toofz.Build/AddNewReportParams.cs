using System.Collections.Generic;

namespace toofz.Build
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddNewReportParams
    {
        /// <summary>
        /// A UUID token used to identify the project.
        /// </summary>
        public string RepositoryUploadToken { get; set; }
        /// <summary>
        /// The target branch for the report. This value may be overridden during the Codecov discovery process.
        /// </summary>
        public string Branch { get; set; }
        /// <summary>
        /// The build number provided by your CI service.
        /// </summary>
        public string Build { get; set; }
        /// <summary>
        /// The job number provided by your CI service.
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// The http url to link back to your CI provider.
        /// </summary>
        public string BuildUrl { get; set; }
        /// <summary>
        /// A custom name for this specific upload.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The owner/repo slug name of the project.
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// The relative path to the codecov.yml in this project.
        /// </summary>
        public string ConfigurationPath { get; set; }
        /// <summary>
        /// The CI service name.
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// Coverage flags used to isolate coverage reports.
        /// </summary>
        public List<string> Flags { get; } = new List<string>();
        /// <summary>
        /// The pull request number this commit is currently found in.
        /// </summary>
        public string PullRequest { get; set; }
    }
}
