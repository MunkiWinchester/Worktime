using System;
using System.Threading.Tasks;
using Worktime.Business;
using Worktime.Views;

namespace Worktime
{
    public static class Core
    {
        private static int UpdateDelay => (int) TimeSpan.FromMinutes(30).TotalMilliseconds;
        public static MainWindow MainWindow { get; set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async void Initialize()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

            UiStyleManager.InitializeTheme();
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
