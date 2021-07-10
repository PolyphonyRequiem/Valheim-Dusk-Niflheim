using System.Collections.Generic;

namespace ModManager
{
    public record NexusModComponent(long ModId, string ModVersion, string FileName, string Description, bool IsDebugTool, List<FileMap> FileMaps, string Extractor = "zip") : ModComponent("nexus", Description, IsDebugTool, FileMaps, Extractor);
}