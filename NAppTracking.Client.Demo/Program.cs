namespace NAppTracking.Client.Demo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SendReport();
        }

        private static async void SendReport()
        {
            var exceptionReport = new ExceptionReport();

            var client = new TrackingClient();

            await client.SendAsync(exceptionReport);
        }
    }
}