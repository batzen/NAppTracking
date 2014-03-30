﻿namespace NAppTracking.Client
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class TrackingClient
    {
        public TrackingClient()
        {
            //this.BaseAddress = new Uri("http://localhost/NAppTracking.Server/");
            this.BaseAddress = new Uri("http://localhost:1856/");
            this.ApiKey = Guid.Empty;
        }

        public Uri BaseAddress { get; set; }

        public Guid ApiKey { get; set; }

        public async Task<bool> SendAsync(ExceptionReport report)
        {
            var exceptionReportAddress = new Uri(this.BaseAddress, Constants.ExceptionReportApiPath);

            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, exceptionReportAddress);

            request.Headers.Add(Constants.ApiKeyHeaderName, this.ApiKey.ToString());

            if (handler.SupportsTransferEncodingChunked())
            {
                request.Headers.TransferEncodingChunked = true;
            }

            request.Content = new StringContent(JsonConvert.SerializeObject(report));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}