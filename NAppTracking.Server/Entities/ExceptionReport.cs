namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class ExceptionReport : IEntity, ICreatedUtc
    {
        public ExceptionReport()
        {
            if (this.ExceptionReportFiles == null) this.ExceptionReportFiles = new HashSet<ExceptionReportFile>();
        }

        public int Id { get; set; }

        public DateTime? CreatedUtc { get; set; }

        [IgnoreDataMember]
        public virtual TrackingApplication Application { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportFile> ExceptionReportFiles { get; set; }
    }
}