using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Tomlyn;

namespace Packkit.Manifest;

public class PackManifest
{
    public Header Header { get; set; } = new Header();
    public Customization Customization { get; set; } = new Customization();
    public Dictionary<string, ModEntry> Mods { get; set; } = [];

    public static PackManifest LoadExisting(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"[MANIFEST] [ERROR-001] No existing manifest found",
                path
            );
        }

        var model = Toml.ToModel<PackManifest>(File.ReadAllText(path));
        return model;
    }

    public static PackManifest LoadOrCreate(string path)
    {
        PackManifest model;

        if (File.Exists(path))
        {
            // Load manifest from path
            model = Toml.ToModel<PackManifest>(File.ReadAllText(path));
        }
        else
        {
            GD.Print(
                $"[MANIFEST] [WARN] Manifest not found: {path}, defaulting to base-manifest.toml"
            );

            // Create new manifest.toml from base-manifest copy
            model = Defaults.CreateManifestFromBase();
        }

        return model;
    }

    public void SaveToFile(string path)
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
