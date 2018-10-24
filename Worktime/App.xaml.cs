using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;
using Microsoft.Win32;
using Worktime.Business;

namespace Worktime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.ReconfigFileTarget();

            // track possible exceptions
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            // add custom accent and theme resource dictionaries to the ThemeManager
            ThemeManager.AddAccent("Spotify",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/Spotify.xaml"));
            ThemeManager.AddAccent("Raspberry",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/Raspberry.xaml"));
            ThemeManager.AddAccent("FireOrange",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/FireOrange.xaml"));
            ThemeManager.AddAppTheme("BaseGray",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Theme/BaseGray.xaml"));

            // start the application
            Core.Initialize();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            Logger.Error(args.Exception.Message, args.Exception);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
        {
            Logger.Error(args.Exception.Message, args.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Logger.Error(e.Message, e);
        }

        private bool ShouldUpdateTheme()
            => DateTime.Now - _lastUpdate >= _updateDelay;
        private TimeSpan _updateDelay = new TimeSpan(0, 0, 30);
        private DateTime _lastUpdate = DateTime.MinValue;

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if ((e.Category == UserPreferenceCategory.Color
                || e.Category == UserPreferenceCategory.General)
                && UiTheme.CurrentAccent.Name == UiTheme.WindowAccentName
                && ShouldUpdateTheme())
            {
                UiTheme.CreateWindowsAccentStyle(true);
                _lastUpdate = DateTime.Now;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Because this is a static event, detach event handler when application is closed, or memory leaks will result.
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        }
    }
}