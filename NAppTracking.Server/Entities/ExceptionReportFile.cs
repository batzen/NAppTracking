namespace NAppTracking.Server.Entities
{
    using System;

    public class ExceptionReportFile : IEntity, ICreatedUtc
    {
        public int Id { get; set; }

        public DateTime CreatedUtc { get; set; }

        public Guid StorageId { get; set; }

        public virtual ExceptionReport ExceptionReport { get; set; }

        public string DisplayName { get; set; }

        public string FileName { get; set; }

        public string MIMEType { get; set; }        

        /// <summary>
        /// Size in bytes
        /// </summary>
        public long? Size { get; set; }
    }
}