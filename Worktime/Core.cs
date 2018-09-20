using System;
using System.Threading.Tasks;
using Worktime.Business;
using Worktime.Updater;
using Worktime.Views;

namespace Worktime
{
    public static class Core
    {
        private static int UpdateDelay => (int) TimeSpan.FromSeconds(10).TotalMilliseconds;
        public static MainWindow MainWindow { get; set; }

        public static async void Initialize()
        {
#if !DEBUG
            var splashScreenWindow = new SplashScreenWindow();
            var updateCheck = Updater.Updating.Updater.StartupUpdateCheck(splashScreenWindow);
            while (!updateCheck.IsCompleted)
            {
                await Task.Delay(500);
            }
#endif

            UiTheme.InitializeTheme();
            MainWindow = new MainWindow();
            MainWindow.LoadConfigSettings();
            MainWindow.Show();
#if !DEBUG
            splashScreenWindow.Close();

            UpdateOverlayAsync();
#endif
        }

        private static async void UpdateOverlayAsync()
        {
            Updater.Updating.Updater.CheckForUpdates(true);

            while(true)
            {
                Updater.Updating.Updater.CheckForUpdates();
                await Task.Delay(UpdateDelay);
            }
        }
    }
}
