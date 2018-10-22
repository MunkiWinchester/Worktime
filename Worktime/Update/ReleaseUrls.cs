namespace Worktime.Update
{
    public class ReleaseUrls
    {
        private const string FieldLive = "live";

        public string Live => "https://github.com/munkiwinchester/worktime";

        public string GetReleaseUrl(string release) => Live;
    }
}
