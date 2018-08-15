﻿using System;

namespace Worktime.Extension
{
    public static class TimeSpanExtension
    {
        public static string ToFormatedString(this TimeSpan timeSpan)
        {
            return $@"{(timeSpan < TimeSpan.Zero ? "- " : "")}{
                        Math.Abs((int)timeSpan.TotalHours):00}:{Math.Abs(timeSpan.Minutes):00}";
        }
    }
}
