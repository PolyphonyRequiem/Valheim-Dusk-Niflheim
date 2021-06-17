using NexusModsNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModManager
{
    public class NexusModComponentHandler : ComponentHandlerBase<NexusModComponent>
    {
        private InfosInquirer infosInquirer;

        public NexusModComponentHandler(INexusModsClient client, Dictionary<string, ExtractorBase> extractors, DirectoryInfo archiveDownloadRoot, DirectoryInfo componentRoot)
            : base(extractors, archiveDownloadRoot, componentRoot)
        {
            infosInquirer = new InfosInquirer(client);
        }

        public override async Task Resolve(NexusModComponent component)
        {
            Console.WriteLine($"Resolving component {component.Description} using {nameof(NexusModComponentHandler)}");
            
            var fileToFetch = (await infosInquirer.ModFiles.GetModFilesAsync("Valheim", component.ModId))
                .ModFiles
                .Where(f => f.Category != NexusModsNET.DataModels.NexusModFileCategory.Deleted)
                .Where(f => f.ModVersion == component.ModVersion)
                .Where(f => f.Name == component.FileName)
                .First();

            var links = await infosInquirer.ModFiles.GetModFileDownloadLinksAsync("Valheim", component.ModId, fileToFetch.FileId);
            var archive = await DownloadFileAsync(component, links.First().Uri.ToString(), fileToFetch.FileName);

            base.Extract(component, archive);
        }
    }
}
