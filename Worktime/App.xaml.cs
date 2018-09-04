using System;
using System.Windows;
using MahApps.Metro;

namespace Worktime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // add custom accent and theme resource dictionaries to the ThemeManager
            ThemeManager.AddAccent("Spotify",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/Spotify.xaml"));
            ThemeManager.AddAccent("Raspberry",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/Raspberry.xaml"));
            ThemeManager.AddAccent("FireOrange",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Accent/FireOrange.xaml"));
            ThemeManager.AddAppTheme("BaseGray",
                new Uri("pack://application:,,,/Worktime;component/Resources/Custom/Theme/BaseGray.xaml"));

            Core.Initialize();
            //var splash = new Updater.SplashScreenWindow();
            //splash.Show();
            //
            //Updater.GitHub.CheckForUpdate("MunkiWinchester", "Worktime", new Version(0, 0, 0, 0)).Wait();
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
        }
    }
}