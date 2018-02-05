using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace toofz.Build
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CodecovClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public CodecovClient()
        {
            http = new HttpClient();
        }

        private readonly HttpClient http;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public async Task<AddNewReportResponse> AddNewReportAsync(
            string commit,
            AddNewReportParams @params = null)
        {
            @params = @params ?? new AddNewReportParams();

            var requestUri = "https://codecov.io/upload/v4";
            var queryParams = new Dictionary<string, string>
            {
                ["commit"] = commit,
                ["token"] = @params.RepositoryUploadToken,
                ["branch"] = @params.Branch,
                ["build"] = @params.Build,
                ["job"] = @params.Job,
                ["build_url"] = @params.BuildUrl,
                ["name"] = @params.Name,
                ["slug"] = @params.Slug,
                ["yaml"] = @params.ConfigurationPath,
                ["service"] = @params.Service,
                ["pr"] = @params.PullRequest,
            };
            if (@params.Flags.Count > 0)
            {
                queryParams.Add("flags", string.Join(",", @params.Flags));
            }
            var query = ToQueryString(queryParams);
            requestUri += query;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            var response = await http.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(content);
            }
            var urls = content.Split('\n');

            return new AddNewReportResponse(urls[0], urls[1]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> UploadReportAsync(
            Uri requestUri,
            string report)
        {
            var content = new StringContent(report);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            content.Headers.Add("x-amz-acl", "public-read");
            content.Headers.Add("x-amz-storage-class", "REDUCED_REDUNDANCY");

            return http.PutAsync(requestUri, content);
        }

        private string ToQueryString(IDictionary<string, string> @params)
        {
            var queryParams = new List<string>();

            foreach (var param in @params)
            {
                if (param.Value != null)
                {
                    queryParams.Add($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
                }
            }

            return "?" + string.Join("&", queryParams);
        }

        #region IDisposable Implementation

        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (disposed) { return; }

            http.Dispose();

            disposed = true;
        }

        #endregion
    }
}
