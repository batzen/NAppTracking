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
            //this.BaseAddress = new Uri("http://localhost/NAppTracking.Server/");
            //this.BaseAddress = new Uri("http://localhost.fiddler:1856/");

            this.BaseAddress = baseAddress;
            this.ApiKey = apiKey;

            this.exceptionReportAddress = new Uri(this.BaseAddress, Constants.ExceptionReportApiPath);
        }

        public Uri BaseAddress { get; private set; }

        public Guid ApiKey { get; private set; }

        public async Task<bool> SendAsync(Exception ex, HashSet<ExceptionReportCustomDataSetDto> customData = null, HashSet<ExceptionReportFileDto> exceptionReportFiles = null)
        {
            return await this.SendAsync(new ExceptionReportDto(ex, customData, exceptionReportFiles));
        }

        public async Task<bool> SendAsync(ExceptionReportDto report)
        {
            this.EnsureExceptionReportHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, this.exceptionReportAddress);

            this.AddApiKeyHeader(request);

            request.Content = new StringContent(JsonConvert.SerializeObject(report));
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            try
            {
                var response = await this.exceptionReportHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    return await this.SendExceptionReportFiles(response.Headers.Location, report.ExceptionReportFiles);
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private async Task<bool> SendExceptionReportFiles(Uri reportUri, IEnumerable<ExceptionReportFileDto> reportFiles)
        {
            reportFiles = reportFiles.ToList();
            if (reportFiles.Any() == false)
            {
                return true;
            }

            var request = new HttpRequestMessage(HttpMethod.Put, reportUri);

            this.AddApiKeyHeader(request);

            var content = new MultipartFormDataContent();
            
            foreach (var file in reportFiles)
            {
                content.Add(new StreamContent(new FileStream(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)), file.Name, file.FileName);   
            }            

            request.Content = content;

            try
            {
                var response = await this.exceptionReportHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
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