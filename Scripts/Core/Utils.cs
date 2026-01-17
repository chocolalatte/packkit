using System;
using System.IO;
using System.Security.Cryptography;
using Packkit.Manifest;
using Packkit.Tags;
using Tomlyn;

namespace Packkit.Core;

public static class Utils
{
    public static string Hash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        byte[] hashBytes = sha.ComputeHash(stream);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    // TODO: Check if still needed
    public static void SaveBaseManifest(string path = "../base-manifest-generated.toml")
    {
        var manifest = CreateBaseManifest();
        string tomlText = Toml.FromModel(manifest);

        File.WriteAllText(path, tomlText);
        Console.WriteLine($"[UTILS] [INFO] Base manifest saved at {path}");
    }

    private static PackManifest CreateBaseManifest()
    {
        Customization.TagRegistry tagRegistry = new()
        {
            SimpleTags =
            [
                new TagDefinitions.SimpleTagDefinition
                {
                    Name = "favorite",
                    Description = "Toggle favorite mods",
                },
            ],
            ValueTags =
            [
                new TagDefinitions.ValueTagDefinition
                {
                    Name = "priority",
                    Description =
                        "Priority of the mod, lower number means higher priority, default is 0",
                    DefaultValue = 0,
                },
            ],
            EnumTags =
            [
                new TagDefinitions.EnumTagDefinition
                {
                    Name = "importance",
                    Description = "Importance of the mod",
                    Options = ["required", "recommended", "optional"],
                },
                new TagDefinitions.EnumTagDefinition
                {
                    Name = "impact",
                    Description = "Impact of the mod",
                    Options = ["light", "heavy", "performance"],
                },
            ],
        };
        return new PackManifest
        {
            Header = new Header
            {
                SchemaVersion = "2.4.0",
                PackVersion = "1.0.0",
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                LastUpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            },
            Mods = [],
            Customization = new Customization { Tags = tagRegistry },
        };
    }
}
