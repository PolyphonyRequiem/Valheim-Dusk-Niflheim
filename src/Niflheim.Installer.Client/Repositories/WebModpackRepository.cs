using Niflheim.Core.Repository;
using Niflheim.Installer.Clients;
using Niflheim.Installer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niflheim.Installer.Client.Repositories
{
    public class WebModpackRepository : ModpackRepository<ModpackArchiveDefinition>
    {
        private readonly JsonModpackClient<ModpackArchiveDefinition> client;

        public WebModpackRepository(JsonModpackClient<ModpackArchiveDefinition> client)
        {
            this.client = client;
        }

        public override List<ModpackArchiveDefinition> GetAllActiveModpacks()
        {
            return client.GetModpacksAsync().Result;
        }

        public override List<ModpackArchiveDefinition> GetAllActiveModpacksWithTag(string tag)
        {
            return client.GetModpacksAsync().Result
                         .Where(m=>String.Equals(m.Tag, tag, StringComparison.InvariantCultureIgnoreCase))
                         .ToList();
        }

        public override List<string> GetAllActiveModpackTags()
        {
            return client.GetModpacksAsync().Result
                         .Select(m=>m.Tag)
                         .Distinct(StringComparer.InvariantCultureIgnoreCase)
                         .ToList();
        }
    }
}
