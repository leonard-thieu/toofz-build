using System;

namespace toofz.Build
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddNewReportResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportUri"></param>
        /// <param name="s3Uri"></param>
        public AddNewReportResponse(string reportUri, string s3Uri)
        {
            if (reportUri == null)
                throw new ArgumentNullException(nameof(reportUri));
            if (s3Uri == null)
                throw new ArgumentNullException(nameof(s3Uri));

            ReportUri = new Uri(reportUri);
            S3Uri = new Uri(s3Uri);
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri ReportUri { get; }
        /// <summary>
        /// 
        /// </summary>
        public Uri S3Uri { get; }
    }
}
