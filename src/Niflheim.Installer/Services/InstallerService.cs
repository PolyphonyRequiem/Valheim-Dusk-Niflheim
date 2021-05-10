using Niflheim.Installer.Entities;
using Niflheim.Installer.Extensions;
using System.IO;
using System.Text.Json;

namespace Niflheim.Installer.Services
{
    public class InstallerService
    {
        private const string modInfoFileName = "niflheim.json";
        private readonly DirectoryInfo valheimDirectoryInfo;
        private readonly DirectoryInfo niflheimDirectoryInfo;

        public InstallerService(DirectoryInfo valheimDirectoryInfo, DirectoryInfo niflheimDirectoryInfo)
        {
            this.valheimDirectoryInfo = valheimDirectoryInfo;
            this.niflheimDirectoryInfo = niflheimDirectoryInfo;
        }


        public void InstallOrUpdate(ModpackArchiveDefinition modpack)
        {
            if (TryUpdate(modpack))
            {
                //Update succeeded.
            }
            else if (TryCleanInstall(modpack))
            {
                //Clean install succeeded.
            }
        }

        public string GetModVersion(DirectoryInfo niflheimDirectoryInfo)
        {
            return GetModInfo().Version;
        }

        private bool TryCleanInstall(ModpackArchiveDefinition modpack)
        {
            if (niflheimDirectoryInfo.Exists)
            {
                Directory.Delete(niflheimDirectoryInfo.FullName, true);
            }

            this.valheimDirectoryInfo.Copy(niflheimDirectoryInfo, true);

            ExtractModpack(modpack);

            WriteModInfo(new ModInfo { Version = modpack.Version });

            return true;
        }

        private bool TryUpdate(ModpackArchiveDefinition modpack)
        {
            if (!niflheimDirectoryInfo.Exists)
            {
                return false;
            }

            var modInfo = GetModInfo();

            if (modInfo.Equals(ModInfo.None))
            {
                return false;
            }

            ExtractModpack(modpack);

            return true;
        }

        private void ExtractModpack(ModpackArchiveDefinition modpack)
        {
            var archiveInfo = ModpackArchiveDownloader.GetReleaseArchiveAsync(modpack).Result;

            System.IO.DirectoryInfo bepinexDirectoryInfo = new System.IO.DirectoryInfo(Path.Combine(niflheimDirectoryInfo.FullName, "BepInEx"));
            System.IO.DirectoryInfo bepinexDirectoryInfoBackup = new System.IO.DirectoryInfo(Path.Combine(niflheimDirectoryInfo.FullName, "BepInEx_backup"));

            if (bepinexDirectoryInfoBackup.Exists)
            {
                Directory.Delete(bepinexDirectoryInfoBackup.FullName, true);
            }

            if (bepinexDirectoryInfo.Exists)
            {
                Directory.Move(bepinexDirectoryInfo.FullName, bepinexDirectoryInfoBackup.FullName);
            }

            ArchiveExtractor.ExtractArchive(archiveInfo, niflheimDirectoryInfo);
        }

        private ModInfo GetModInfo()
        {
            FileInfo modInfoFileInfo = new FileInfo(Path.Combine(niflheimDirectoryInfo.FullName, modInfoFileName));

            if (!modInfoFileInfo.Exists)
            {
                return ModInfo.None;
            }

            string modInfoFileContent = File.ReadAllText(modInfoFileInfo.FullName);

            return JsonSerializer.Deserialize<ModInfo>(modInfoFileContent, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }


        private void WriteModInfo(ModInfo modInfo)
        {
            FileInfo modInfoFileInfo = new FileInfo(Path.Combine(niflheimDirectoryInfo.FullName, modInfoFileName));

            File.WriteAllText(modInfoFileInfo.FullName, JsonSerializer.Serialize<ModInfo>(modInfo, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

    }
}
