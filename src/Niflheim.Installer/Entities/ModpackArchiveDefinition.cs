using Niflheim.Core.Entities;
using System;

namespace Niflheim.Installer.Entities
{
    public class ModpackArchiveDefinition : ModpackDefinition
    {
        public Uri ArchiveUri { get; init;  }
    }
}
