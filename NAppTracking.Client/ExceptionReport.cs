namespace NAppTracking.Client
{
    public class ExceptionReport
    {
        public ExceptionReport()
        {
            this.ExceptionType = string.Empty;
            this.ExceptionMessage = string.Empty;            
            this.StackTrace = string.Empty;
        }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }
    }
}