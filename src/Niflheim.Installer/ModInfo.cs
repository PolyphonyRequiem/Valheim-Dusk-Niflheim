namespace Niflheim.Installer
{
    public class ModInfo
    {
        public string Version { get; init; }

        public static ModInfo None = new ModInfo { Version = "" };
    }
}