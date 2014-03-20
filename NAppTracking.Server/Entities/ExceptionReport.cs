namespace NAppTracking.Server.Entities
{
    public class ExceptionReport
    {
        public int Key { get; set; }

        public TrackingApplication Application { get; set; }

        public int ApplicationKey { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }
    }
}