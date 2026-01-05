using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core;

public class Manifest
{
    public Header Header { get; set; } = new Header();
    public Dictionary<string, ModEntry> Mods { get; set; } = [];

    public static Manifest LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"[MANIFEST] [ERROR-001] Manifest not found: {path}");

        string tomlText = File.ReadAllText(path);
        return Toml.ToModel<Manifest>(tomlText);
    }

    public void SaveToFile(string path)
    {
        string tomlText = Toml.FromModel(this);

        var lines = tomlText.Split('\n');
        var formattedLines = new List<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("[") && formattedLines.Count > 0)
                formattedLines.Add("");

            formattedLines.Add(line);
        }

        string formattedToml = string.Join("\n", formattedLines);

        File.WriteAllText(path, formattedToml);
    }
}

public class Header
{
    public string SchemaVersion { get; set; } = ManifestUtils.GetSchemaVersionFromEmbedded();
    public string PackVersion { get; set; } = "1.0.0";
}

public static class ManifestUtils
{
    public static string GetSchemaVersionFromEmbedded()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Packkit.Core.base-manifest.toml";

        using Stream? stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException(
                $"[MANIFEST] [ERROR-002] Embedded resource '{resourceName}' not found."
            );
        using StreamReader reader = new(stream);
        string tomlText = reader.ReadToEnd();

        TomlTable model = Toml.ToModel(tomlText);
        if (model.TryGetValue("header", out var headerObj) && headerObj is TomlTable header)
        {
            if (header.TryGetValue("schema_version", out var schema))
            {
                return schema.ToString();
            }
        }

        throw new Exception("[MANIFEST] [ERROR-003] schema_version not found in embedded manifest");
    }
}
