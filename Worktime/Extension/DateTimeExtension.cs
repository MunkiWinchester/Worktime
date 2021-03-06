﻿using System;
using System.Globalization;
using Worktime.DataObjetcs;

namespace Worktime.Extension
{
    public static class DateTimeExtension
    {
        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static IsoWeek Iso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                time = time.AddDays(3);

            // Return the week of our adjusted day
            return new IsoWeek(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday), time.Year);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var end = StartOfWeek(dt, startOfWeek).AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59)
                .AddMilliseconds(999);
            return end;
        }

        public static DateTime ToTimelessDateTime(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static TimeSpan ToDatelessTimeSpan(this DateTime dateTime)
        {
            return new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}