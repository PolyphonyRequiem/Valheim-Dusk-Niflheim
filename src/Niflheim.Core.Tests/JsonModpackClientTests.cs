using Niflheim.Installer.Clients;
using Niflheim.Installer.Entities;
using System;
using System.Linq;
using Xunit;

namespace Niflheim.Core.Tests
{
    public class JsonModpackClientTests
    {
        private JsonModpackClient<ModpackArchiveDefinition> client;
        private Uri modpacksUri = new Uri("https://niflheim.blob.core.windows.net/modpacksdiscovery/modpackarchives.json");

        public JsonModpackClientTests()
        {
            this.client = new JsonModpackClient<ModpackArchiveDefinition>(this.modpacksUri);
        }

        [Fact]
        public void GetModpacksTest()
        {
            var modpacks = this.client.GetModpacksAsync().Result;
            var expected = new Uri("https://niflheim.blob.core.windows.net/modpacks/testmodpack.zip");
            Assert.Equal(modpacks.First().ArchiveUri, expected);
        }
    }
}
