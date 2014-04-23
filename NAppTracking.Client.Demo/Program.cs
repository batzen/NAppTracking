namespace NAppTracking.Client.Demo
{
    using System;
    using System.Collections.Generic;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var client = new TrackingClient(new Uri("http://batzen/NAppTracking.Server/"), Guid.Empty);
            //var client = new TrackingClient(new Uri("http://localhost.fiddler:1856/"), Guid.Empty);        

            try
            {
                try
                {
                    ThrowException();
                }
                catch (Exception exception)
                {
                    var t = client.SendAsync(exception, 
                        new HashSet<ExceptionReportCustomDataSetDto> 
                        { 
                            new ExceptionReportCustomDataSetDto("Custom")
                                {
                                    CustomData = new HashSet<ExceptionReportCustomDataDto>
                                    {
                                         new ExceptionReportCustomDataDto("Test-Key1", "Test-Value1"),
                                         new ExceptionReportCustomDataDto("Test-Key2", "Test-Value2"),
                                         new ExceptionReportCustomDataDto("Test-Key3", "Test-Value3"),
                                         new ExceptionReportCustomDataDto("Test-Key4", "Test-Value4"),
                                         new ExceptionReportCustomDataDto("Test-Key5", "Test-Value5"),
                                    }
                                }
                        },
                        new HashSet<ExceptionReportFileDto>
                        {
                            new ExceptionReportFileDto("Demo.txt")
                        });

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

        private static void ThrowException()
        {
            try
            {
                try
                {
                    throw new Exception("Root cause");
                }
                catch (Exception exception)
                {
                    throw new Exception("Just a try/catch test", exception);
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Outermost exception", exception);
            }
        }
    }
}