using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ModManager
{
    public abstract class ComponentHandlerBase<T> where T:ModComponent
    {
        protected readonly Dictionary<string, ExtractorBase> extractors;
        protected readonly DirectoryInfo archiveDownloadRoot;
        protected readonly DirectoryInfo componentRoot;

        public ComponentHandlerBase(Dictionary<string, ExtractorBase> extractors, DirectoryInfo archiveDownloadRoot, DirectoryInfo componentRoot)
        {
            this.extractors = extractors;
            this.archiveDownloadRoot = archiveDownloadRoot;
            this.componentRoot = componentRoot;

            if (this.archiveDownloadRoot.Exists)
            {
                this.archiveDownloadRoot.Delete(true);
            }
            this.archiveDownloadRoot.Create();

            if (this.componentRoot.Exists)
            {
                this.componentRoot.Delete(true);
            }
            this.componentRoot.Create();
        }

        public abstract Task Resolve(T component);

        protected async Task<FileInfo> DownloadFileAsync(ModComponent component, string downloadUri, string filename)
        {
            using (WebClient webClient = new WebClient())
            {
                var outFile = new FileInfo(Path.Combine(archiveDownloadRoot.FullName, filename));
                Console.WriteLine($"\tDownloading component {component.Description} from {downloadUri}");
                await webClient.DownloadFileTaskAsync(downloadUri, outFile.FullName);
                Console.WriteLine($"\tDownloaded to {outFile.FullName}");
                return outFile;
            }
        }

        protected void Extract(ModComponent component, FileInfo archive)
        {
            if (extractors.ContainsKey(component.Extractor))
            {
                extractors[component.Extractor].Extract(archive, archiveDownloadRoot, componentRoot, component.FileMaps);
            }
        }
    }
}