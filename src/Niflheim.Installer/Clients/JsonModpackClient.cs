using Niflheim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Niflheim.Installer.Clients
{
    public class JsonModpackClient<T> where T : ModpackDefinition
    {
        private readonly Uri modpacksUri;

        public JsonModpackClient(Uri modpacksUri)
        {
            this.modpacksUri = modpacksUri;
        }

        public async Task<List<T>> GetModpacksAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string modpacksJson = await client.GetStringAsync(this.modpacksUri);

                return JsonSerializer.Deserialize<List<T>>(modpacksJson, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true                    
                });
            }
        }
    }
}
