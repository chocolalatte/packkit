#if false
using System.Xml.Linq;
using Packkit.Manifest;

namespace Packkit.Tags;

public static class Tagger
{
    public static void AddSimpleTag(
        string tag,
        ModEntry modEntry,
        PackManifest manifest,
        bool save = true
    )
    {
        var simpleTagDefinitions = manifest.Customization.Tags.SimpleTags;

        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (!simpleTagDefinitions.Any(t => t.Name == tag))
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag \"{tag}\" not found in manifest");
            return;
        }
        // Check if tag is already applied to mod
        // Return and log warning if it is
        if (modEntry.Tags.Simple.Contains(tag))
        {
            Console.WriteLine(
                $"[MANIFEST:TAGGER] [WARN] Tag \"{tag}\" already exists in \"{modEntry.Name}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Simple.Add(tag);

        Console.WriteLine($"[MANIFEST:TAGGER] [INFO] Added tag \"{tag}\" to \"{modEntry.Name}\"");

        // For future helper functions
        if (save)
            manifest.SaveToFile();
    }

    public static void AddValueTag(
        string tag,
        object value,
        ModEntry modEntry,
        PackManifest manifest,
        bool save = true
    )
    {
        var valueTagDefinitions = manifest.Customization.Tags.ValueTags;

        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (!valueTagDefinitions.Any(t => t.Name == tag))
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag \"{tag}\" not found in manifest");
            return;
        }

        var normalizedValue = NormalizeValueTagObject(
            valueTagDefinitions.First(t => t.Name == tag),
            value
        );
        // Check if tag already has same value
        // Return and log warning if it does
        if (
            modEntry.Tags.Value.TryGetValue(tag, out object? existingValue)
            && existingValue.Equals(normalizedValue)
        )
        {
            Console.WriteLine(
                $"[MANIFEST:TAGGER] [WARN] Tag \"{tag}\" already exists in \"{modEntry.Name}\" with value \"{normalizedValue}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Value[tag] = normalizedValue;
        Console.WriteLine(
            $"[MANIFEST:TAGGER] [INFO] Added value tag \"{tag}\" with value of \"{normalizedValue}\" to \"{modEntry.Name}\""
        );

        // For future helper functions
        if (save)
            manifest.SaveToFile();
    }

    // Convert object to a TOML supported type
    private static object NormalizeValueTagObject(
        TagDefinitions.ValueTagDefinition definition,
        object value
    )
    {
        return definition.Type switch
        {
            TagDefinitions.ValueTagType.Integer => Int64.Parse(value.ToString()!),
            TagDefinitions.ValueTagType.Float => float.Parse(value.ToString()!),
            TagDefinitions.ValueTagType.Boolean => bool.Parse(value.ToString()!),
            TagDefinitions.ValueTagType.String => value,
            _ => value,
        };
    }

    public static void AddEnumTag(
        string tag,
        string value,
        ModEntry modEntry,
        PackManifest manifest,
        bool save = true
    )
    {
        var enumTagDefinitions = manifest.Customization.Tags.EnumTags;
        var enumTag = enumTagDefinitions.FirstOrDefault(t => t.Name == tag);

        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (enumTag == null)
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag \"{tag}\" not found in manifest");
            return;
        }
        // Check if tags options contains value
        // Return and log error if it doesn't
        else if (!enumTag.Options.Contains(value))
        {
            Console.WriteLine(
                $"[MANIFEST:TAGGER] [ERROR-002] Tag \"{tag}\" does not contain possible value \"{value}\""
            );
            return;
        }
        // Run if manifest contains definition for tag and contains value
        // Check if tag already has same value
        if (
            modEntry.Tags.Enum.TryGetValue(tag, out string? existingValue)
            && existingValue == value
        )
        {
            Console.WriteLine(
                $"[MANIFEST:TAGGER] [WARN] Tag \"{tag}\" already exists in \"{modEntry.Name}\" with value \"{value}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Enum[tag] = value;
        Console.WriteLine(
            $"[MANIFEST:TAGGER] [INFO] Added enum tag \"{tag}\" with value of \"{value}\" to \"{modEntry.Name}\""
        );
        // For future helper functions
        if (save)
            manifest.SaveToFile();
    }
}
#endif
