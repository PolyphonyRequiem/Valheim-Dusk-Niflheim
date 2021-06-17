using System.Collections.Generic;

namespace ModManager
{
    public record ModComponent(string Type, string Description, List<FileMap> FileMaps, string Extractor="zip");
}