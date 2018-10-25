using System;

namespace Worktime.DataObjetcs
{
    public class OnChangeEventArgs : EventArgs
    {
        public string PropertyName { get; set; }

        public OnChangeEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public Employee Employee { get; set; }
        public double Percent { get; set; }

        public ProgressEventArgs(Employee employee, double percent)
        {
            Employee = employee;
            Percent = percent;
        }
    }

    public class RunningStateEventArgs : EventArgs
    {
        public bool IsRunning { get; set; }

        public RunningStateEventArgs(bool isRunning)
        {
            IsRunning = isRunning;
        }
    }

    public class StyleChangeEventArgs : EventArgs
    {
        public MahApps.Metro.Accent Accent { get; set; }
        public MahApps.Metro.AppTheme Theme { get; set; }

        public StyleChangeEventArgs(MahApps.Metro.Accent accent, MahApps.Metro.AppTheme theme)
        {
            Accent = accent;
            Theme = theme;
        }
    }
}
