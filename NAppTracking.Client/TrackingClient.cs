namespace NAppTracking.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class TrackingClient
    {
        private HttpClient exceptionReportHttpClient;
        private readonly Uri exceptionReportAddress;

        public TrackingClient(Uri baseAddress, Guid apiKey)
        {
            this.BaseAddress = baseAddress;
            this.ApiKey = apiKey;

            this.exceptionReportAddress = new Uri(this.BaseAddress, Constants.ExceptionReportApiPath);
        }

        public Uri BaseAddress { get; private set; }

        public Guid ApiKey { get; private set; }

        public async Task<TrackingResult> SendAsync(Exception ex, string comment = null, HashSet<ExceptionReportCustomDataSetDto> customData = null, HashSet<ExceptionReportFileDto> exceptionReportFiles = null)
        {
            return await this.SendAsync(new ExceptionReportDto(ex, comment, customData, exceptionReportFiles));
        }

        public async Task<TrackingResult> SendAsync(ExceptionReportDto report)
        {
            this.EnsureExceptionReportHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, this.exceptionReportAddress);

            this.AddApiKeyHeader(request);
            
            var content = new StringContent(JsonConvert.SerializeObject(report));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            request.Content = content;

            try
            {
                var response = await this.exceptionReportHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var result = new TrackingResult(response);

                if (response.IsSuccessStatusCode)
                {
                    return result.Combine(await this.SendExceptionReportFiles(response.Headers.Location, report.ExceptionReportFiles));
                }

                return result;
            }
            catch (Exception exception)
            {
                return new TrackingResult(exception);
            }
        }

        public async Task<TrackingResult> SendExceptionReportFiles(Uri reportUri, IEnumerable<ExceptionReportFileDto> reportFiles)
        {
            reportFiles = reportFiles.ToList();
            if (reportFiles.Any() == false)
            {
                return new TrackingResult();
            }

            var request = new HttpRequestMessage(HttpMethod.Put, reportUri);

            this.AddApiKeyHeader(request);

            var content = new MultipartFormDataContent();
            
            AddFilesToContent(reportFiles, content);            

            request.Content = content;

            try
            {
                var response = await this.exceptionReportHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                return new TrackingResult(response);
            }
            catch (Exception exception)
            {
                return new TrackingResult(exception);
            }
        }

        private static void AddFilesToContent(IEnumerable<ExceptionReportFileDto> reportFiles, MultipartFormDataContent content)
        {
            foreach (var file in reportFiles)
            {
                content.Add(new StreamContent(new FileStream(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)), Path.GetFileName(file.FullPath), Path.GetFileName(file.FullPath));
            }
        }

        private void AddApiKeyHeader(HttpRequestMessage request)
        {
            request.Headers.Add(Constants.ApiKeyHeaderName, this.ApiKey.ToString());
        }

        private void EnsureExceptionReportHttpClient()
        {
            var handler = new HttpClientHandler();
            this.exceptionReportHttpClient = new HttpClient(handler, true);
        }
    }
}