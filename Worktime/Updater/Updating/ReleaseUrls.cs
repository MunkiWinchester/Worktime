using Newtonsoft.Json;

namespace Worktime.Updater.Updating
{
    public class ReleaseUrls
    {
        private const string FieldLive = "live";

        [JsonProperty(FieldLive)]
        public string Live { get; set; }

        public string GetReleaseUrl(string release) => Live;
    }
}
