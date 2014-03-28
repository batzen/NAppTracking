namespace NAppTracking.Client
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class TrackingClient
    {
        public TrackingClient()
        {
            this.Address = "http://localhost:1856/";
            this.ApiKey = Guid.Empty;
        }

        public string Address { get; set; }

        public Guid ApiKey { get; set; }

        public async Task<bool> SendAsync(ExceptionReport report)
        {           
            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, Path.Combine(this.Address, Constants.ExceptionReportApiPath));

            request.Headers.Add(Constants.ApiKeyHeaderName, this.ApiKey.ToString());

            if (handler.SupportsTransferEncodingChunked())
            {
                request.Headers.TransferEncodingChunked = true;
            }

            var response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}