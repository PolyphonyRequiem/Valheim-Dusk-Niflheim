using NexusModsNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModManager
{
    public class ModManifestResolver
    {
        private readonly string nexusApiKey;

        public ModManifestResolver(string nexusApiKey)
        {
            if (string.IsNullOrWhiteSpace(nexusApiKey))
            {
                throw new ArgumentException($"'{nameof(nexusApiKey)}' cannot be null or whitespace.", nameof(nexusApiKey));
            }

            this.nexusApiKey = nexusApiKey;
        }
        public async Task ResolveManifest(FileInfo manifestFile, DirectoryInfo componentRoot, DirectoryInfo archiveDownloadRoot)
        {
            var client = NexusModsClient.Create(this.nexusApiKey);
            var extractors = new Dictionary<string, ExtractorBase>
            {
                {"zip", new ZipExtractor() },
                {"rar", new RarExtractor() },
                {"7z", new SevenZipExtractor() },
                {"none", new NoExtractor() }
            };

            NexusModComponentHandler nexusHandler = new NexusModComponentHandler(client, extractors, archiveDownloadRoot, componentRoot);
            HttpModComponentHandler httpHandler = new HttpModComponentHandler(extractors, archiveDownloadRoot, componentRoot);

            Manifest manifest = JsonSerializer.Deserialize<Manifest>(
                File.ReadAllText(manifestFile.FullName),
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new ModComponentJsonConverter() }
                })!;

            foreach (var component in manifest.Components)
            {
                await (component switch
                {
                    NexusModComponent nexusComponent => nexusHandler.Resolve(nexusComponent),
                    HttpModComponent httpComponent => httpHandler.Resolve(httpComponent),
                    _ => throw new NotImplementedException("Unable to handle unknown component type")
                });
            }

            Process.Start("cmd", $"/C tree {componentRoot.FullName} /f").WaitForExit();
        }
    }
}