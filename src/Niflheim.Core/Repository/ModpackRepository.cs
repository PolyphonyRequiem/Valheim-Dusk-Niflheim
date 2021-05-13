using Niflheim.Core.Entities;
using System.Collections.Generic;

namespace Niflheim.Core.Repository
{
    public abstract class ModpackRepository<T> where T : ModpackDefinition
    {
        public abstract List<T> GetAllActiveModpacksWithTag(string tag);

        public abstract List<T> GetAllActiveModpacks();

        public abstract List<string> GetAllActiveModpackTags();
    }
}
