using System.IO.Compression;
using System.Text.Json;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core;

public class Parser
{
    public static ModEntry? ParseForge(ZipArchiveEntry modsToml, string filePath)
    {
        using StreamReader reader = new(modsToml.Open());
        string content = reader.ReadToEnd();

        TomlTable model = Toml.ToModel(content);
        var modsArray =
            model.TryGetValue("mods", out var modsObj) && modsObj is TomlTableArray array
                ? array
                : null;

        if (modsArray == null || modsArray.Count == 0)
        {
            return null;
        }

        var mod = (TomlTable)modsArray[0];

        ModEntry modEntry = new()
        {
            ModId = mod.TryGetValue("modId", out var id) ? id.ToString() : null,
            Name = mod.TryGetValue("displayName", out var name) ? name.ToString() : null,
            File = Path.GetFileName(filePath),
            Version = mod.TryGetValue("version", out var ver) ? ver.ToString() : null, // Fix issue with forge version showing "{file.jarVersion}"
            Loader = ModLoader.forge,
            Side = ModSide.unknown,
        };

        return modEntry;
    }

    public static ModEntry? ParseFabric(ZipArchiveEntry modsJson, string filePath)
    {
        using StreamReader reader = new(modsJson.Open());
        string json = reader.ReadToEnd();

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        string? id = root.TryGetProperty("id", out var idProperty) ? idProperty.GetString() : null;

        string? name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

        string? version = root.TryGetProperty("version", out var verProp)
            ? verProp.GetString()
            : null;

        ModSide side = ModSide.unknown;
        if (root.TryGetProperty("environment", out var environmentProperty))
        {
            side = environmentProperty.GetString() switch
            {
                "client" => ModSide.client,
                "server" => ModSide.server,
                "*" => ModSide.both,
                _ => ModSide.unknown,
            };
        }

        ModEntry modEntry = new()
        {
            ModId = id,
            Name = name,
            Version = version,
            File = Path.GetFileName(filePath),
            Loader = ModLoader.fabric,
            Side = side,
        };

        if (
            root.TryGetProperty("depends", out var depends)
            && depends.ValueKind == JsonValueKind.Object
        )
        {
            foreach (var depend in depends.EnumerateObject())
            {
                modEntry.Requires.Add(depend.Name);
            }
        }

        if (
            root.TryGetProperty("reccomends", out var reccomends)
            && reccomends.ValueKind == JsonValueKind.Object
        )
        {
            foreach (var recomend in reccomends.EnumerateObject())
            {
                modEntry.Recommends.Add(recomend.Name);
            }
        }

        return modEntry;
    }
}
