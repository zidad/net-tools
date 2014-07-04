using System;

namespace Net.System
{ 
    public static class DateTimeExtensions 
    {
        public static bool Between(this DateTime date, DateTime start, DateTime end)
        {
            return date <= end && date >= start;
        }

        public static bool Between(this DateTime? date, DateTime? start, DateTime? end)
        {
            if(!date.HasValue)
                return false;
            
            return date <= end && date >= start;
        }
    }
}