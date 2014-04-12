namespace NAppTracking.Client.Demo
{
    using System;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var client = new TrackingClient();

            try
            {
                try
                {
                    throw new Exception("Just a try/catch test");
                }
                catch (Exception ex)
                {
                    var t = client.SendAsync(ex);

                    t.Wait();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadLine();
                return 1;
            }

            return 0;
        }
    }
}