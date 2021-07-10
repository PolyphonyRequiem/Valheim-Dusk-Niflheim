using System.Collections.Generic;

namespace ModManager
{
    public record ModComponent(string Type, string Description, bool isDebugTool, List<FileMap> FileMaps, string Extractor="zip");
}