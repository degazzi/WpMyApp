using System;

namespace WpMyApp.Helpers
{
    public static class DateTimeExtensions
    {
        public static int DaysUntil(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return int.MaxValue;
            var dt = dateTime.Value.Date;
            return (dt - DateTime.UtcNow.Date).Days;
        }

        public static string ToShortDateOrEmpty(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("dd.MM.yyyy") : string.Empty;
        }
    }
}
