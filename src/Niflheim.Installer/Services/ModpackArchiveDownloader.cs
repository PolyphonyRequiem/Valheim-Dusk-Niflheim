using System.Threading.Tasks;
using System.IO;
using System.Net;
using Niflheim.Installer.Entities;

namespace Niflheim.Installer.Services
{
    public static class ModpackArchiveDownloader
    {
        public static Task<FileInfo> GetReleaseArchiveAsync(ModpackArchiveDefinition modpackArchiveDefinition) => GetReleaseArchiveAsync(modpackArchiveDefinition, new FileInfo(@".\niflheim.zip"));

        public static async Task<FileInfo> GetReleaseArchiveAsync(ModpackArchiveDefinition modpackArchiveDefinition, FileInfo outputArchive)
        {
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(modpackArchiveDefinition.ArchiveUri, outputArchive.FullName);
                return outputArchive;
            }
        }
    }
}
