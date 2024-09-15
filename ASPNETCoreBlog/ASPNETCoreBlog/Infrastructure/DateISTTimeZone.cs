namespace ASPNETCoreBlog.Infrastructure
{
    public static class DateISTTimeZone
    {
        public static DateTime GetDateTimeWithCustomTimeZone(string timeZone = "India Standard Time")
        {
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, istZone);
            return istTime;
        }
    }
}
