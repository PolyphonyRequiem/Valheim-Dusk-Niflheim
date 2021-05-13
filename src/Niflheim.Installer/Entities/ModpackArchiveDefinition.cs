using Niflheim.Core.Entities;
using System;

namespace Niflheim.Installer.Entities
{
    public class ModpackArchiveDefinition : ModpackDefinition
    {
        public Uri ArchiveUri { get; init;  }

        public static new ModpackArchiveDefinition None = new ModpackArchiveDefinition
        {
            Id = "none",
            Description = "Not a modpack",
            Version = "",
            Tag = "",
            Disabled = true,
            ArchiveUri=new Uri("http://localhost")
        };
    }
}
