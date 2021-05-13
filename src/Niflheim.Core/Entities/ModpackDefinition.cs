using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niflheim.Core.Entities
{
    public class ModpackDefinition
    {
        public string Id { get; init; } = "";

        public string Description { get; init; } = "";

        public string Version { get; init; } = "";

        public string Tag { get; init; } = "";

        public bool Disabled { get; init; } = false;

        public static ModpackDefinition None = new ModpackDefinition {
            Id="none",
            Description="Not a modpack",
            Version="",
            Tag="",
            Disabled=true
        };
    }
}
