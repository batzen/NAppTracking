namespace NAppTracking.Server.Entities
{
    using System;

    public class ExceptionReportFile : IEntity, ICreatedUtc
    {
        public virtual int Id { get; set; }

        public virtual DateTime? CreatedUtc { get; set; }

        public virtual Guid StorageId { get; set; }

        public virtual ExceptionReport ExceptionReport { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string FileName { get; set; }

        public virtual string MIMEType { get; set; }        

        /// <summary>
        /// Size in bytes
        /// </summary>
        public virtual long? Size { get; set; }
    }
}