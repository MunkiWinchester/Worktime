using System;
using System.IO;
using System.Reflection;

namespace Worktime.Business
{
    internal static class Helper
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

            var result = actualTime.TotalMilliseconds / regularTime.TotalMilliseconds;
            result = result * 100;
            return result;
        }

        public static string GetExecutingDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string GetApplicationDataDirectory
        {
            get
            {
                var dir = $@"{Directory.GetParent(Path.GetDirectoryName(
                            Assembly.GetExecutingAssembly().Location)).FullName}\Data";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static Version GetCurrentVersion() => Assembly.GetExecutingAssembly().GetName().Version;

        public static string ToVersionString(this Version version, bool includeRef = false) =>
            $"{version?.Major}.{version?.Minor}.{version?.Build}{(includeRef ? "." + version?.Revision : "")}";
    }
}