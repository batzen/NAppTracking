namespace NAppTracking.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text;

    public class ExceptionReportDto
    {
        public ExceptionReportDto()
        {
        }

        public ExceptionReportDto(Exception exception, HashSet<ExceptionReportCustomDataSetDto> customData = null, HashSet<ExceptionReportFileDto> exceptionReportFiles = null)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Exception = exception;

            var causingException = GetCausingException(exception);

            this.Type = causingException.GetType().ToString();
            this.Message = causingException.Message;

            // Use original exception for details because ToString of that exception contains the details (stacktrace etc.) in the correct order
            this.Details = exception.ToString();

            this.ProcessName = Process.GetCurrentProcess().ProcessName;

            this.Machine = Environment.MachineName;

            this.CustomDataSets = customData ?? new HashSet<ExceptionReportCustomDataSetDto>();

            this.ExceptionReportFiles = exceptionReportFiles ?? new HashSet<ExceptionReportFileDto>();
        }

        private Exception GetCausingException(Exception exception)
        {
            var causingException = exception;

            while (causingException.InnerException != null)
            {
                causingException = causingException.InnerException;
            }

            return causingException;
        }

        [IgnoreDataMember]
        public Exception Exception { get; protected set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public string ProcessName { get; set; }

        public string Machine { get; set; }

        public HashSet<ExceptionReportCustomDataSetDto> CustomDataSets { get; set; }

        [IgnoreDataMember]
        public HashSet<ExceptionReportFileDto> ExceptionReportFiles { get; set; }
    }

    public class ExceptionReportCustomDataSetDto
    {
        public ExceptionReportCustomDataSetDto(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public ICollection<ExceptionReportCustomDataDto> CustomData { get; set; }
    }

    public class ExceptionReportCustomDataDto
    {
        public ExceptionReportCustomDataDto(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class ExceptionReportFileDto
    {
        public ExceptionReportFileDto(string fullPath)
        {       
            this.FullPath = fullPath;
        }

        public string FullPath { get; set; }
    }
}