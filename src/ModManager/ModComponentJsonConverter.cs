using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModManager
{
    public class ModComponentJsonConverter : JsonConverter<ModComponent>
    {
        public override ModComponent Read(
           ref Utf8JsonReader reader,
           Type typeToConvert,
           JsonSerializerOptions options)
        {
            using (var document = JsonDocument.ParseValue(ref reader))
            {
                if(!document.RootElement.TryGetProperty("type", out var typeProperty))
                {
                    throw new JsonException();
                }

                return typeProperty.GetString() switch
                {
                    "nexus" => (ModComponent)JsonSerializer.Deserialize(document.RootElement.GetRawText(), typeof(NexusModComponent), options)!,
                    "http" => (ModComponent)JsonSerializer.Deserialize(document.RootElement.GetRawText(), typeof(HttpModComponent), options)!,
                    string s => throw new JsonException($"Unable to determine discriminator 'type', expected 'nexus' or 'http', found '{s}'")
                };
            }

        }

        public override void Write(Utf8JsonWriter writer, ModComponent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}