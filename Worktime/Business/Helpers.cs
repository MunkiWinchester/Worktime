using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Worktime.DataObjetcs;

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

        public static bool IsWindows10()
        {
            try
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return reg != null && ((string)reg.GetValue("ProductName")).Contains("Windows 10");
            }
            catch (Exception ex)
            {
                Logger.Error("IsWindows10()", ex);
                return false;
            }
        }

        public static bool IsWindows8()
        {
            try
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                return reg != null && ((string)reg.GetValue("ProductName")).Contains("Windows 8");
            }
            catch (Exception ex)
            {
                Logger.Error("IsWindows8()", ex);
                return false;
            }
        }

        public static string LoadAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static TimeSpan CalculateRegularBreak(Employee employee)
        {
            return new TimeSpan(0, Convert.ToInt32(Math.Floor(
                employee.WorkTimeReal <= employee.WorkTimeRegular
                    ? employee.WorkTimeRegular.TotalHours
                    : employee.WorkTimeReal.TotalHours / 3) * 15),
                0);
        }
    }
}