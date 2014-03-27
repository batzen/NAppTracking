namespace NAppTracking.Server.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public static class ApiKeyHelper
    {
        public static Guid GetApiKey(HttpRequestMessage request)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues(Constants.ApiKeyHeaderName, out values))
            {
                var apiKeyHeader = values.FirstOrDefault();
                Guid apiKey;

                if (Guid.TryParse(apiKeyHeader, out apiKey))
                {
                    return apiKey;
                }
            }

            return Guid.Empty;
        }
    }
}