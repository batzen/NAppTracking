namespace NAppTracking.Server.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class ExceptionReport : IEntity
    {
        [Key]
        public int Id { get; set; }

        public virtual TrackingApplication Application { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }
    }
}