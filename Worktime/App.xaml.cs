using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;
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

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Logger.Error(e.Message, e);
        }
    }
}