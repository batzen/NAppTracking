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
            if (this.CustomDataSets == null) this.CustomDataSets = new HashSet<ExceptionReportCustomDataSet>();
        }

        public int Id { get; set; }

        [Display(Name = "Time")]
        public DateTime CreatedUtc { get; set; }

        [IgnoreDataMember]
        public virtual TrackingApplication Application { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public string Comment { get; set; }

        public string ProcessName { get; set; }

        public string Machine { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportFile> ExceptionReportFiles { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportCustomDataSet> CustomDataSets { get; set; }
    }

    public class ExceptionReportCustomDataSet : IEntity
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        public string Name { get; set; }

        [IgnoreDataMember]
        public virtual ExceptionReport ExceptionReport { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportCustomData> CustomData { get; set; }
    }

    public class ExceptionReportCustomData : IEntity
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        [IgnoreDataMember]
        public virtual ExceptionReportCustomDataSet DataSet { get; set; }
    }
}