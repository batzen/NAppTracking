namespace NAppTracking.Server.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime? dt)
        {
            return dt.HasValue 
                ? dt.Value.ToRelativeTime()
                : "Unkown";
        }

        public static string ToRelativeTime(this DateTime dt)
        {
            var utcNow = DateTime.UtcNow;
            return dt <= utcNow 
                ? ToRelativeTimePast(dt, utcNow) 
                : ToRelativeTimeFuture(dt, utcNow);
        }

        private static string ToRelativeTimePast(DateTime dt, DateTime utcNow)
        {
            var ts = utcNow - dt;
            var delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "1 sec ago" : ts.Seconds + " secs ago";
            }

            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "1 min ago" : ts.Minutes + " mins ago";
            }

            if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "1 hour ago" : ts.Hours + " hours ago";
            }

            var days = ts.Days;

            if (days == 1)
            {
                return "yesterday";
            }

            if (days <= 2)
            {
                return days + " days ago";
            }

            if (days <= 330)
            {
                return dt.ToString("MMM %d 'at' %H:mmm");
            }

            return dt.ToString(@"MMM %d \'yy 'at' %H:mmm");
        }

        private static string ToRelativeTimeFuture(DateTime dt, DateTime utcNow)
        {
            var ts = dt - utcNow;
            var delta = ts.TotalSeconds;

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "in 1 second" : "in " + ts.Seconds + " seconds";
            }
            if (delta < 3600) // 60 mins * 60 sec
            {
                return ts.Minutes == 1 ? "in 1 minute" : "in " + ts.Minutes + " minutes";
            }
            if (delta < 86400) // 24 hrs * 60 mins * 60 sec
            {
                return ts.Hours == 1 ? "in 1 hour" : "in " + ts.Hours + " hours";
            }

            // use our own rounding so we can round the correct direction for future
            var days = (int)Math.Round(ts.TotalDays, 0);

            if (days == 1)
            {
                return "tomorrow";
            }

            if (days <= 10)
            {
                return "in " + days + " day" + (days > 1 ? "s" : "");
            }

            if (days <= 330)
            {
                return "on " + dt.ToString("MMM %d 'at' %H:mmm");
            }

            return "on " + dt.ToString(@"MMM %d \'yy 'at' %H:mmm");
        }
    }
}