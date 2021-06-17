using System.Collections.Generic;

namespace ModManager
{
    public record HttpModComponent(string FileUri, string Description, List<FileMap> FileMaps, string Extractor = "zip") : ModComponent("http", Description, FileMaps, Extractor);
}