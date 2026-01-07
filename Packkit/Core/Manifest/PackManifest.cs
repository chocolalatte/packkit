using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core.Manifest;

public class PackManifest
{
    public Header Header { get; set; } = new Header();
    public Customization Customization { get; set; } = new Customization();
    public Dictionary<string, ModEntry> Mods { get; set; } = [];

    public void Initialize()
    {
        Mods ??= new Dictionary<string, ModEntry>();

        foreach (var mod in Mods.Values)
        {
            mod.Tags ??= new ModTags();

            mod.Tags.Simple ??= new List<string>();
            mod.Tags.Value ??= new Dictionary<string, object>();
            mod.Tags.Enum ??= new Dictionary<string, string>();

            mod.Notes ??= new List<string>();
            mod.Requires ??= new List<string>();
            mod.Recommends ??= new List<string>();
        }
    }

    public static PackManifest LoadFromFile(string path)
    {
        PackManifest model;

        if (File.Exists(path))
        {
            // Load manifest from path
            model = Toml.ToModel<PackManifest>(File.ReadAllText(path));
        }
        else
        {
            Console.WriteLine(
                $"[MANIFEST] [WARN] Manifest not found: {path}, defaulting to base-manifest.toml"
            );

            // Create new manifest.toml from base-manifest copy
            model = Defaults.CreateManifestFromBase();
        }

        model.Initialize();
        return model;
    }

    public void SaveToFile(string path = "../manifest.toml")
    {
        Header.Touch();

        string tomlText = Toml.FromModel(this);

        string[] lines = tomlText.Split('\n');
        List<string> formattedLines = [];

        // Add newlines between sections
        foreach (var line in lines)
        {
            if (line.StartsWith('[') && formattedLines.Count > 0)
                formattedLines.Add("");

            formattedLines.Add(line);
        }

        string formattedToml = string.Join("\n", formattedLines);

        File.WriteAllText(path, formattedToml);
    }
}
