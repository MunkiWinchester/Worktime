using System;

namespace Worktime.Extension
{
    public static class TimeSpanExtension
    {
        public static string ToFormatedString(this TimeSpan timeSpan, bool keepItUnder24Hrs = false)
        {
            if(keepItUnder24Hrs)
                return $@"{(timeSpan < TimeSpan.Zero ? "- " : "")}{
                            Math.Abs(timeSpan.Hours):00}:{Math.Abs(timeSpan.Minutes):00}";
            else
                return $@"{(timeSpan < TimeSpan.Zero ? "- " : "")}{
                            Math.Abs(timeSpan.TotalHours):00}:{Math.Abs(timeSpan.Minutes):00}";
        }
    }
}
