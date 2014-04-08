namespace NAppTracking.Client.Demo
{
    using System;

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

            try
            {
                var t = client.SendAsync(exceptionReport);

                t.Wait();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.ReadLine();
        }
    }
}