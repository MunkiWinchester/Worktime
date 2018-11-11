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

        public static DirectoryInfo GetJsonSaveLocation()
        {
            DirectoryInfo dirInfo = null;
            var location = Path.Combine(GetBaseSaveDirectory(), DateTime.Now.Year.ToString());
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
            dirInfo = new DirectoryInfo(location);
            return dirInfo;
        }

        public static string GetBaseSaveDirectory()
        {
            var location = string.Empty;
#if DEBUG
            location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Test");
#else
            location = Path.Combine(
                            Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                            $".{Assembly.GetExecutingAssembly().GetName().Name}");
#endif
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
            return location;
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
