using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Squirrel;

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
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<string> GetReleaseUrl(string release)
        {
            if(_releaseUrls != null)
                return _releaseUrls.GetReleaseUrl(release);
            var file = Path.Combine(AppDataPath, "releases.json");
            string fileContent;
            try
            {
                Console.WriteLine("Downloading releases file");
                using(var wc = new WebClient())
                    await wc.DownloadFileTaskAsync("https://hsdecktracker.net/releases.json", file);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            using(var sr = new StreamReader(file))
                fileContent = sr.ReadToEnd();
            _releaseUrls = JsonConvert.DeserializeObject<ReleaseUrls>(fileContent);
            var url = _releaseUrls.GetReleaseUrl(release);
            Console.WriteLine($"using '{release}' release: {url}");
            return url;
        }

        private static async Task<UpdateManager> GetUpdateManager()
        {
            return await UpdateManager.GitHubUpdateManager(await GetReleaseUrl("live"));
        }

        public static async Task StartupUpdateCheck(SplashScreenWindow splashScreenWindow)
        {
            try
            {
                Console.WriteLine("Checking for updates");
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

                if(updated)
                {
                    if(splashScreenWindow.SkipUpdate)
                    {
                        Console.WriteLine("Update complete, showing update bar");
                        StatusBar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Console.WriteLine("Update complete, restarting");
                        UpdateManager.RestartApp();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
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
                    Console.WriteLine($"Could not move ExecutionStub. {e}");
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
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<bool> SquirrelUpdate(UpdateManager mgr, SplashScreenWindow splashScreenWindow, bool ignoreDelta = false)
        {
            try
            {
                Console.WriteLine($"ignoreDelta={ignoreDelta})");
                var updateInfo = await mgr.CheckForUpdate(ignoreDelta);
                if(!updateInfo.ReleasesToApply.Any())
                {
                    Console.WriteLine("No new updated available");
                    return false;
                }
                var latest = updateInfo.ReleasesToApply.LastOrDefault()?.Version;
                var current = mgr.CurrentlyInstalledVersion();
                if(latest <= current)
                {
                    Console.WriteLine($"{latest}). Not downloading updates.");
                    return false;
                }
                if(IsRevisionIncrement(current?.Version, latest?.Version))
                {
                    Console.WriteLine($"{latest}) is revision increment. Updating in background.");
                    if(splashScreenWindow != null)
                        splashScreenWindow.SkipUpdate = true;
                }
                Console.WriteLine($"{(ignoreDelta ? "" : "delta ")}releases, latest={latest?.Version}");
                if(splashScreenWindow != null)
                    await mgr.DownloadReleases(updateInfo.ReleasesToApply, splashScreenWindow.Updating);
                else
                    await mgr.DownloadReleases(updateInfo.ReleasesToApply);
                splashScreenWindow?.Updating(100);
                Console.WriteLine("Applying releases");
                if(splashScreenWindow != null)
                    await mgr.ApplyReleases(updateInfo, splashScreenWindow.Installing);
                else
                    await mgr.ApplyReleases(updateInfo);
                splashScreenWindow?.Installing(100);
                await mgr.CreateUninstallerRegistryEntry();
                Console.WriteLine("Done");
                return true;
            }
            catch(Exception ex)
            {
                if(ignoreDelta)
                    return false;
                if(ex is Win32Exception)
                    Console.WriteLine("Not able to apply deltas, downloading full release");
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
            Console.WriteLine("Restarting...");
            UpdateManager.RestartApp();
        }
    }
}
