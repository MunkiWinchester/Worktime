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
        protected override void OnStartup(StartupEventArgs e)
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

            base.OnStartup(e);
        }
    }
}