namespace NAppTracking.Server.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class ExceptionReport : IEntity, ICreatedUtc
    {
        public virtual int Id { get; set; }

        public virtual DateTime? CreatedUtc { get; set; }

        [IgnoreDataMember]
        public virtual TrackingApplication Application { get; set; }

        public virtual string ExceptionType { get; set; }

        public virtual string ExceptionMessage { get; set; }

        public virtual string StackTrace { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ExceptionReportFile> ExceptionReportFiles { get; set; }
    }
}