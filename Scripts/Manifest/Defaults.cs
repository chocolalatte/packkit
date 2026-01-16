using System;
using System.IO;
using System.Reflection;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Manifest;

public static class Defaults
{
    public const string BaseManifestPath = "Scripts/Manifest/base-manifest.toml";

    private static string? _baseManifest = null;
    private static string? _schemaVersion = null;

    public static string BaseManifest => _baseManifest ??= GetBaseManifest();
    public static string SchemaVersion => _schemaVersion ??= GetSchemaVersionFromEmbedded();

    private static string GetBaseManifest()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream =
            File.OpenRead(BaseManifestPath)
            ?? throw new FileNotFoundException(
                $"[MANIFEST:DEFAULTS] [ERROR-001] Embedded resource '{BaseManifestPath}' not found."
            );

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    public static PackManifest CreateManifestFromBase()
    {
        PackManifest manifest = Toml.ToModel<PackManifest>(BaseManifest);
        manifest.Header.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        manifest.Header.LastUpdatedAt = manifest.Header.CreatedAt;
        return manifest;
    }

    private static string GetSchemaVersionFromEmbedded()
    {
        string tomlText = GetBaseManifest();
        // Parse the base-manifest and try to get the schema_version from the header
        TomlTable model = Toml.ToModel(tomlText);
        if (model.TryGetValue("header", out var headerObject) && headerObject is TomlTable header)
        {
            if (header.TryGetValue("schema_version", out var schema))
            {
                return schema.ToString()!;
            }
        }

        return "unknown";
        throw new Exception(
            $"[MANIFEST:DEFAULTS] [ERROR-002] schema_version not found in embedded manifest: {BaseManifestPath}"
        );
    }
}
