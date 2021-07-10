using System.Collections.Generic;

namespace ModManager
{
    public record HttpModComponent(string FileUri, string Description, bool IsDebugTool, List<FileMap> FileMaps, string Extractor = "zip") : ModComponent("http", Description, IsDebugTool, FileMaps, Extractor);
}