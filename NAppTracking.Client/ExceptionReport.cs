namespace NAppTracking.Client
{
    using System;
    using Newtonsoft.Json;

    public class ExceptionReport
    {
        public ExceptionReport(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Exception = exception;
            this.ExceptionType = exception.GetType().ToString();
            this.ExceptionMessage = exception.Message;
            this.StackTrace = exception.StackTrace;

            this.ExceptionDetail = exception.ToString();

            this.MachineName = Environment.MachineName;
        }

        [JsonIgnore]
        public Exception Exception { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionDetail { get; set; }

        public string MachineName { get; set; }
    }
}