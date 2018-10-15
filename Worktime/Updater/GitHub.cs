using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Worktime.Business;

namespace Worktime.Updater
{
    public class GitHub
    {
        // TODO: Use this with zip..
        public static async Task<Release> CheckForUpdate(string user, string repo, Version version)
        {
            try
            {
                Logger.Info($"{user}/{repo}: Checking for updates (current={version})");
                var latest = await GetLatestRelease(user, repo);
                if (latest != null && latest.Assets.Count > 0)
                {
                    if (latest.GetVersion()?.CompareTo(version) > 0)
                    {
                        Logger.Info($"{user}/{repo}: A new version is available (latest={latest.Tag})");
                        return latest;
                    }
                    Logger.Info($"{user}/{repo}: We are up-to-date (latest={latest.Tag})");
                }
            }
            catch (Exception e)
            {
                Logger.Error("CheckForUpdate(string user, string repo, Version version)", e);
            }
            return null;
        }

        private static async Task<Release> GetLatestRelease(string user, string repo)
        {
            try
            {
                string json;
                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, user);
                    var url = $"https://api.github.com/repos/{user}/{repo}/releases/latest";
                    json = await wc.DownloadStringTaskAsync(url);
                }
                return JsonConvert.DeserializeObject<Release>(json);
            }
            catch (Exception e)
            {
                Logger.Error("GetLatestRelease(string user, string repo)", e);
            }
            return null;
        }

        public static async Task<string> DownloadRelease(Release release, string downloadDirectory)
        {
            try
            {
                var path = Path.Combine(downloadDirectory, release.Assets[0].Name);
                using (var wc = new WebClient())
                    await wc.DownloadFileTaskAsync(release.Assets[0].Url, path);
                return path;
            }
            catch (Exception e)
            {
                Logger.Error("DownloadRelease(Release release, string downloadDirectory)", e);
                return null;
            }
        }

        public class Release
        {
            [JsonProperty("tag_name")]
            public string Tag { get; set; }

            [JsonProperty("assets")]
            public List<Asset> Assets { get; set; }

            public class Asset
            {
                [JsonProperty("browser_download_url")]
                public string Url { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }
            }

            public Version GetVersion() => Version.TryParse(Tag.Replace("v", ""), out Version v) ? v : null;
        }
    }
}
