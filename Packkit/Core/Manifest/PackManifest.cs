using Tomlyn;

namespace Packkit.Core.Manifest;

public class PackManifest
{
    public Header Header { get; set; } = new Header();
    public Customization Customization { get; set; } = new Customization();
    public Dictionary<string, ModEntry> Mods { get; set; } = [];

    public static PackManifest LoadFromFile(string path)
    {
        string tomlText;

        if (File.Exists(path))
        {
            tomlText = File.ReadAllText(path);
        }
        else
        {
            Console.WriteLine(
                $"[MANIFEST] [WARN] Manifest not found: {path}, defaulting to base-manifest.toml"
            );

            try
            {
                tomlText = Toml.FromModel(CreateManifestFromBase());
            }
            catch (Exception exception)
            {
                throw new FileNotFoundException(
                    $"[MANIFEST] [ERROR-001] base-manifest not found: {Defaults.BaseManifestResourceName}",
                    Defaults.BaseManifestResourceName,
                    exception
                );
            }
        }

        return Toml.ToModel<PackManifest>(tomlText);
    }

    public static PackManifest CreateManifestFromBase()
    {
        return Toml.ToModel<PackManifest>(Defaults.BaseManifest);
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
