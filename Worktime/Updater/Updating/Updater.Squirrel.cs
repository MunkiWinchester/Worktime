using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;
using Worktime.Business;

namespace Worktime.Updater.Updating
{
    internal static partial class Updater
    {
        private static ReleaseUrls _releaseUrls;
        private static TimeSpan _updateCheckDelay = new TimeSpan(0, 20, 0);
        private static bool ShouldCheckForUpdates()
            => DateTime.Now - _lastUpdateCheck >= _updateCheckDelay;
        private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetExecutingAssembly().GetName().Name);

        public static async void CheckForUpdates(bool force = false)
        {
            if(!force && !ShouldCheckForUpdates())
                return;
            _lastUpdateCheck = DateTime.Now;
            try
            {
                bool updated;
                using(var mgr = await GetUpdateManager())
                    updated = await SquirrelUpdate(mgr, null);

                if(updated)
                {
                    _updateCheckDelay = new TimeSpan(1, 0, 0);
                    StatusBar.Visibility = Visibility.Visible;
                }
            }
            catch(Exception e)
            {
                Logger.Error("CheckForUpdates(bool force = false)", e);
            }
        }

        private static string GetReleaseUrl(string release)
        {
            if (_releaseUrls != null)
                return _releaseUrls.GetReleaseUrl(release);
            else
                _releaseUrls = new ReleaseUrls();
            var url = _releaseUrls.GetReleaseUrl(release);
            Logger.Info($"using '{release}' release: {url}");
            return url;
        }

        private static async Task<UpdateManager> GetUpdateManager()
        {
            // https://github.com/Squirrel/Squirrel.Windows/blob/master/docs/using/github.md
            return await UpdateManager.GitHubUpdateManager(GetReleaseUrl("live"));
        }

        public static async Task StartupUpdateCheck(SplashScreenWindow splashScreenWindow)
        {
            try
            {
                Logger.Info("Checking for updates");
                bool updated;
                using(var mgr = await GetUpdateManager())
                {
                    SquirrelAwareApp.HandleEvents(
                        v =>
                        {
                            mgr.CreateShortcutForThisExe();
                        },
                        v =>
                        {
                            mgr.CreateShortcutForThisExe();
                            FixStub();
                        },
                        onAppUninstall: v =>
                        {
                            mgr.RemoveShortcutForThisExe();
                        },
                        onFirstRun: CleanUpInstallerFile
                        );
                    updated = await SquirrelUpdate(mgr, splashScreenWindow);
                }

                if (updated)
                {
                    Logger.Info("Update complete, restarting");
                    UpdateManager.RestartApp();
                }
            }
            catch(Exception e)
            {
                Logger.Error("StartupUpdateCheck(SplashScreenWindow splashScreenWindow)", e);
            }
        }

        public static void FixStub()
        {
            var dir = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
            var stubPath = Path.Combine(dir, "HearthstoneDeckTracker_ExecutionStub.exe");
            if(File.Exists(stubPath))
            {
                var newStubPath = Path.Combine(Directory.GetParent(dir).FullName, "HearthstoneDeckTracker.exe");
                try
                {
                    File.Move(stubPath, newStubPath);
                }
                catch(Exception e)
				{
					Logger.Error("Could not move ExecutionStub.", e);
                }
            }
        }

        private static void CleanUpInstallerFile()
        {
            try
            {
                var file = Path.Combine(AppDataPath, "HDT-Installer.exe");
                if(File.Exists(file))
                    File.Delete(file);
            }
            catch(Exception e)
			{
				Logger.Error("CleanUpInstallerFile()", e);
            }
        }

        private static async Task<bool> SquirrelUpdate(UpdateManager mgr, SplashScreenWindow splashScreenWindow, bool ignoreDelta = false)
        {
            try
            {
                var updateInfo = await mgr.CheckForUpdate(ignoreDelta);
                if(!updateInfo.ReleasesToApply.Any())
                {
                    Logger.Info("No new updated available");
                    return false;
                }
                var latest = updateInfo.ReleasesToApply.LastOrDefault()?.Version;
                var current = mgr.CurrentlyInstalledVersion();
                if(latest <= current)
                {
					Logger.Info($"{latest}). Not downloading updates.");
                    return false;
                }
                if(IsRevisionIncrement(current?.Version, latest?.Version))
                {
					Logger.Info($"{latest}) is revision increment. Updating in background.");
                }
				Logger.Info($"{(ignoreDelta ? "" : "delta ")}releases, latest={latest?.Version}");
                if(splashScreenWindow != null)
                    await mgr.DownloadReleases(updateInfo.ReleasesToApply, splashScreenWindow.Updating);
                else
                    await mgr.DownloadReleases(updateInfo.ReleasesToApply);
                splashScreenWindow?.Updating(100);
				Logger.Info("Applying releases");
                if(splashScreenWindow != null)
                    await mgr.ApplyReleases(updateInfo, splashScreenWindow.Installing);
                else
                    await mgr.ApplyReleases(updateInfo);
                splashScreenWindow?.Installing(100);
                await mgr.CreateUninstallerRegistryEntry();
				Logger.Info("Done");
                return true;
            }
            catch(Exception e)
            {
                if(ignoreDelta)
                    return false;
                if(e is Win32Exception)
					Logger.Error("Not able to apply deltas, downloading full release", e);
                return await SquirrelUpdate(mgr, splashScreenWindow, true);
            }
        }

        private static bool IsRevisionIncrement(Version current, Version latest)
        {
            if(current == null || latest == null)
                return false;
            return current.Major == latest.Major && current.Minor == latest.Minor && current.Build == latest.Build
                    && current.Revision < latest.Revision;
        }

        internal static void StartUpdate()
        {
            Logger.Info("Restarting...");
            UpdateManager.RestartApp();
        }
    }
}
