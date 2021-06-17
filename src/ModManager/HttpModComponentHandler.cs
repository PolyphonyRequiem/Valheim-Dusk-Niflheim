using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModManager
{
    public class HttpModComponentHandler : ComponentHandlerBase<HttpModComponent>
    {
        public HttpModComponentHandler(Dictionary<string, ExtractorBase> extractors, DirectoryInfo archiveDownloadRoot, DirectoryInfo componentRoot)
            : base (extractors, archiveDownloadRoot, componentRoot)
        {
        }

        public override async Task Resolve(HttpModComponent component)
        {
            Console.WriteLine($"Resolving component {component.Description} using {nameof(HttpModComponentHandler)}");

            var archiveFileName = Path.Combine(
                archiveDownloadRoot.FullName,
                new FileInfo(Path.ChangeExtension(Path.GetTempFileName(), $".{component.Extractor}")).Name);

            var archive = await DownloadFileAsync(component, component.FileUri, archiveFileName);

            base.Extract(component, archive);
        }
    }
}