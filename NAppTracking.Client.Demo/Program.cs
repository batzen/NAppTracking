namespace NAppTracking.Client.Demo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var exceptionReport = new ExceptionReport();

            var client = new TrackingClient();

            client.SendAsync(exceptionReport).Wait();
        }
    }
}