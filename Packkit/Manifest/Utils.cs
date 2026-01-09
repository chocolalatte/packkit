using System;
using System.ComponentModel;
using System.IO;
using Packkit.Tags;
using Tomlyn;

namespace Packkit.Manifest;

public static class Utils
{
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

    public static void SaveBaseManifest(string path = "../base-manifest-generated.toml")
    {
        var manifest = CreateBaseManifest();
        string tomlText = Toml.FromModel(manifest);

        File.WriteAllText(path, tomlText);
        Console.WriteLine($"[UTILS] [INFO] Base manifest saved at {path}");
    }
}
