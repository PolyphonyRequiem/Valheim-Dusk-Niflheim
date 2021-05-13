namespace Niflheim.Installer
{
    public class ModInfo
    {
        public SemanticVersion Version { get; init; }

        public static ModInfo None = new ModInfo { Version = SemanticVersion.Parse("0.0.0") };
    }
}