using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace toofz.Build
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Codecov : Microsoft.Build.Utilities.Task
    {
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Commit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Branch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Build { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BuildUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remote { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConfigurationPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] Flags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PullRequest { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string[] RepositoryFiles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string[] Results { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                ExecuteAsync().GetAwaiter().GetResult();

                return !Log.HasLoggedErrors;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, showStackTrace: true, showDetail: true, file: null);

                return false;
            }
        }

        private async Task ExecuteAsync()
        {
            using (var client = new CodecovClient())
            {
                if (Slug == null && Remote != null)
                {
                    var remoteUri = new Uri(Remote);
                    if (remoteUri.Host == "github.com")
                    {
                        // LocalPath will be in format `/:owner/:repo.git` (e.g. `/leonard-thieu/toofz-build.git`).
                        // Slug needs to be in format `:owner/:repo` so the leading `/` and `.git` is stripped off.
                        Slug = remoteUri.LocalPath.Substring("/".Length, remoteUri.LocalPath.Length - 1 - ".git".Length);
                    }
                }
                var @params = new AddNewReportParams
                {
                    RepositoryUploadToken = Token,
                    Branch = Branch,
                    Build = Build,
                    Job = Job,
                    BuildUrl = BuildUrl,
                    Name = Name,
                    Slug = Slug,
                    ConfigurationPath = ConfigurationPath,
                    Service = Service,
                    PullRequest = PullRequest,
                };
                if (Flags != null) { @params.Flags.AddRange(Flags); }

                var urls = await client.AddNewReportAsync(Commit, @params).ConfigureAwait(false);
                Log.LogMessage(MessageImportance.High, $"View report at {urls.ReportUri}");
                Log.LogMessage($"Uploading report to {urls.S3Uri}");

                var codecovReport = WriteCodecovReport();

                var uploadResponse = await client.UploadReportAsync(urls.S3Uri, codecovReport).ConfigureAwait(false);
                if (!uploadResponse.IsSuccessStatusCode)
                {
                    Log.LogError($"Failed to upload report: HTTP {(int)uploadResponse.StatusCode} {uploadResponse.ReasonPhrase}.");
                    var uploadContent = await uploadResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Log.LogError(uploadContent);
                }
                else
                {
                    var builder = new UriBuilder(urls.S3Uri);
                    builder.Query = null;
                    Log.LogMessage(MessageImportance.High, $"Uploaded report to {builder.Uri}");
                }
            }
        }

        private string WriteCodecovReport()
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms, UTF8NoBOM, bufferSize: 1024, leaveOpen: true))
            {
                foreach (var repoFilePath in RepositoryFiles)
                {
                    sw.WriteLine(repoFilePath);
                }
                sw.WriteLine("<<<<<< network");

                foreach (var resultPath in Results)
                {
                    sw.WriteLine($"# path={resultPath}");
                    sw.Flush();
                    using (var fs = File.OpenRead(resultPath))
                    {
                        fs.CopyTo(ms);
                        sw.WriteLine();
                    }
                    sw.WriteLine("<<<<<< EOF");
                }

                sw.Flush();

                return Encoding.UTF8.GetString(ms.ToArray()).Replace("\r", "");
            }
        }
    }
}
