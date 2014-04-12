namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class ExceptionReport : IEntity, ICreatedUtc
    {
        public ExceptionReport()
        {
            if (this.ExceptionReportFiles == null) this.ExceptionReportFiles = new HashSet<ExceptionReportFile>();
        }

        public int Id { get; set; }

        [Display(Name = "Time")]
        public DateTime? CreatedUtc { get; set; }

        [IgnoreDataMember]
        public virtual TrackingApplication Application { get; set; }

        [Display(Name = "Type")]
        public string ExceptionType { get; set; }

        [Display(Name = "Message")]
        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        [Display(Name = "Machine")]
        public string MachineName { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportFile> ExceptionReportFiles { get; set; }
    }
}