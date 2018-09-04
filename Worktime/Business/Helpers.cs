using System;
using System.Reflection;

namespace Worktime.Business
{
    internal static class Helpers
    {
        /// <summary>
        /// Returns the calculated percentage of the actual and the regular time
        /// </summary>
        /// <param name="actualTime"></param>
        /// <param name="regularTime"></param>
        /// <returns></returns>
        public static double CalculatePercentage(TimeSpan actualTime, TimeSpan regularTime)
        {
            if (actualTime.TotalMinutes.Equals(0d) || regularTime.TotalMinutes.Equals(0d))
                return 0;

            var result = actualTime.TotalMinutes / regularTime.TotalMinutes;
            result = result * 100;
            return result;
        }

        public static Version GetCurrentVersion() => Assembly.GetExecutingAssembly().GetName().Version;
        public static string ToVersionString(this Version version, bool includeRef = false) =>
            $"{version?.Major}.{version?.Minor}.{version?.Build}{(includeRef ? "." + version?.Revision : "")}";
    }
}