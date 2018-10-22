using System;
using System.Threading.Tasks;
using Worktime.Business;
using Worktime.Views;

namespace Worktime
{
    public static class Core
    {
        private static int UpdateDelay => (int) TimeSpan.FromMinutes(10).TotalMilliseconds;
        public static MainWindow MainWindow { get; set; }

        public static async void Initialize()
        {
#if !DEBUG
            var splashScreenWindow = new SplashScreenWindow();
            splashScreenWindow.Show();
            var updateCheck = Update.Updater.StartupUpdateCheck(splashScreenWindow);
            while (!updateCheck.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
#endif

			UiTheme.InitializeTheme();
            MainWindow = new MainWindow();
            MainWindow.LoadConfigSettings();
            MainWindow.Show();

#if !DEBUG
            // Only close it after opening MainWindow!
            splashScreenWindow.Close();
            UpdateOverlayAsync();
#endif
        }

        private static async void UpdateOverlayAsync()
        {
            Update.Updater.CheckForUpdates(true);

            while(true)
            {
                Update.Updater.CheckForUpdates();
                await Task.Delay(UpdateDelay);
            }
        }
    }
}
