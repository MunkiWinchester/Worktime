using System;

namespace Worktime.Update
{
    internal static partial class Updater
    {
        private static DateTime _lastUpdateCheck = DateTime.Now;
        public static StatusBarHelper StatusBar { get; } = new StatusBarHelper();
    }
}
