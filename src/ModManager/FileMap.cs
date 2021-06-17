namespace ModManager
{
    public record FileMap(string SourceRoot = "./", string Sources="**/*", string Exclusions="", string Destination= "./Bepinex/plugins/");
}