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

    public static ModEntry? ParseFabric(ZipArchiveEntry modsJson, string fileName)
    {
        string content = new StreamReader(modsJson.Open()).ReadToEnd();
        TomlTable model = Toml.ToModel(content);

        using JsonDocument doc = JsonDocument.Parse(content);
        return null;
    }
}
