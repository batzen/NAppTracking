namespace NAppTracking.Client
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

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
            this.ExceptionType = exception.GetType().ToString();
            this.ExceptionMessage = exception.Message;

            this.Details = exception.ToString();

            this.MachineName = Environment.MachineName;

            this.CustomDataSets = customData ?? new HashSet<ExceptionReportCustomDataSetDto>();

            this.ExceptionReportFiles = exceptionReportFiles ?? new HashSet<ExceptionReportFileDto>();
        }

        [IgnoreDataMember]
        public Exception Exception { get; protected set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string Details { get; set; }

        public string MachineName { get; set; }

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
        public ExceptionReportFileDto(string name, string fileName, string fullPath)
        {
            this.Name = name;
            this.FileName = fileName;            
            this.FullPath = fullPath;
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string FullPath { get; set; }
    }
}