using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Niflheim.Installer.Clients
{
    public class DiscoveryClient
    {
        public DiscoveryClient(Uri discoveryUri)
        {
            DiscoveryUri = discoveryUri;
        }

        public Uri DiscoveryUri { get; }

        public UpdateCheckResult CheckForLauncherUpdate(SemanticVersion currentVersion)
        {
            var result = GetDiscoveryResult();

            var latestVersion = SemanticVersion.Parse(result.Launcher.Latest);

            if (latestVersion > currentVersion)
            {
                //update!
                return new UpdateCheckResult(true, latestVersion, result.Launcher.UpdateUrl);
            }
            return new UpdateCheckResult(false, currentVersion, "");
        }

        public Uri GetFeedUrl()
        {
            var result = GetDiscoveryResult();

            return new Uri(result.ModpackFeedUrl);
        }

        private DiscoveryResult GetDiscoveryResult()
        {
            using (WebClient wc = new WebClient())
            {
                DiscoveryResult discoveryResult = JsonSerializer.Deserialize<DiscoveryResult>(wc.DownloadString(DiscoveryUri.ToString()), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });


                return discoveryResult;
            }
        }
    }
}
