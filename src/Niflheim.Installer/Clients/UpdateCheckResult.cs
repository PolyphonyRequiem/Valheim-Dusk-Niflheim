namespace Niflheim.Installer.Clients
{
    public class UpdateCheckResult
    {
        public UpdateCheckResult(bool updateRequired, SemanticVersion latestVersion, string updateUrl)
        {
            this.UpdateRequired = updateRequired;
            LatestVersion = latestVersion;
            UpdateUrl = updateUrl;
        }

        public bool UpdateRequired { get; }
        public SemanticVersion LatestVersion { get; }
        public string UpdateUrl { get; }
    }
}