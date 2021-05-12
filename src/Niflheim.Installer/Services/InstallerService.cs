using Niflheim.Installer.Entities;
using Niflheim.Installer.Extensions;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            if (GetModInfo() == ModInfo.None )
            {
                CleanInstall(modpack);
            }

            if (UpdateNeeded(modpack))
            {
                Update(modpack);
            }
        }

        private bool UpdateNeeded(ModpackArchiveDefinition modpack)
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

            if (modInfo.Version >= SemanticVersion.Parse(modpack.Version))
            {
                return false;
            }
            else
            { 
                return true;
            }
        }

        public SemanticVersion GetModVersion()
        {
            return GetModInfo().Version;
        }

        public void CleanInstall(ModpackArchiveDefinition modpack)
        {
            if (niflheimDirectoryInfo.Exists)
            {
                Directory.Delete(niflheimDirectoryInfo.FullName, true);
            }

            this.valheimDirectoryInfo.Copy(niflheimDirectoryInfo, true);

            ExtractModpack(modpack);
        }

        public void Update(ModpackArchiveDefinition modpack)
        {
            ExtractModpack(modpack);
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

            WriteModInfo(new ModInfo { Version = SemanticVersion.Parse(modpack.Version) });
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
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new SemVerJsonConverter()
                }
            });
        }


        private void WriteModInfo(ModInfo modInfo)
        {
            FileInfo modInfoFileInfo = new FileInfo(Path.Combine(niflheimDirectoryInfo.FullName, modInfoFileName));

            File.WriteAllText(modInfoFileInfo.FullName, JsonSerializer.Serialize<ModInfo>(modInfo, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new SemVerJsonConverter()
                }
            }));
        }

        private class SemVerJsonConverter : JsonConverter<SemanticVersion>
        {
            public override SemanticVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetString();
            }

            public override void Write(Utf8JsonWriter writer, SemanticVersion value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
