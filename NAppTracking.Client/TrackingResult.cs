namespace NAppTracking.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    public class TrackingResult
    {
        public TrackingResult()
        {
            this.Exceptions = new List<Exception>();
            this.ResponseMessages = new List<HttpResponseMessage>();
        }

        public TrackingResult(Exception exception)
            : this()
        {   
            this.Exceptions.Add(exception);
        }

        public TrackingResult(HttpResponseMessage responseMessage)
            : this()
        {
            this.ResponseMessages.Add(responseMessage);
        }

        public bool Success
        {
            get 
            { 
                return this.Exceptions.Any() == false
                    && this.ResponseMessages.All(x => x.IsSuccessStatusCode); 
            }
        }

        public List<Exception> Exceptions { get; private set; }

        public List<HttpResponseMessage> ResponseMessages { get; private set; }

        public TrackingResult Combine(TrackingResult trackingResult)
        {
            var combinedTrackingResult = new TrackingResult();
            combinedTrackingResult.Exceptions.AddRange(this.Exceptions);
            combinedTrackingResult.Exceptions.AddRange(trackingResult.Exceptions);

            combinedTrackingResult.ResponseMessages.AddRange(this.ResponseMessages);
            combinedTrackingResult.ResponseMessages.AddRange(trackingResult.ResponseMessages);

            return combinedTrackingResult;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Success: {0}", this.Success);
            stringBuilder.AppendLine();

            foreach (var exception in this.Exceptions)
            {
                stringBuilder.AppendLine("########################################");
                stringBuilder.AppendLine("Exception:");
                stringBuilder.AppendFormat("Message: {0}", exception.Message);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("Details: {0}", exception);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("########################################");                
            }

            foreach (var responseMessage in this.ResponseMessages)
            {
                var task = responseMessage.Content.ReadAsStringAsync();
                task.Wait();
                stringBuilder.AppendLine("########################################");
                stringBuilder.AppendLine("Response:");
                stringBuilder.AppendFormat("StatusCode: {0}", responseMessage.StatusCode);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("Response: {0}", task.Result);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("########################################");
            }

            return stringBuilder.ToString();
        }
    }
}