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
        if (simpleTagDefinitions.Any(t => t.Name == tag))
        {
            if (modEntry.Tags.Simple.Contains(tag))
            {
                Console.WriteLine(
                    $"[MANIFEST:TAGGER] [WARN] Tag {tag} already exists in {modEntry.Name}"
                );
                return;
            }
            else
            {
                modEntry.Tags.Simple.Add(tag);

                Console.WriteLine($"[MANIFEST:TAGGER] [INFO] Added tag {tag} to {modEntry.Name}");
                if (save)
                    manifest.SaveToFile();
            }
        }
        else
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag {tag} not found in manifest");
        }
    }

    // Convert object to a TOML supported type
    public static object NormalizeValueTagObject(
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
        if (valueTagDefinitions.Any(t => t.Name == tag))
        {
            value = NormalizeValueTagObject(valueTagDefinitions.First(t => t.Name == tag), value);

            // Check if tag already has same value
            if (
                modEntry.Tags.Value.TryGetValue(tag, out object? existingValue)
                && existingValue.Equals(value)
            )
            {
                Console.WriteLine(
                    $"[MANIFEST:TAGGER] [WARN] Tag {tag} already exists in {modEntry.Name} with value {value}"
                );
                return;
            }
            modEntry.Tags.Value[tag] = value;
            Console.WriteLine(
                $"[MANIFEST:TAGGER] [INFO] Added value tag {tag} with value of {value} to {modEntry.Name}"
            );
            if (save)
                manifest.SaveToFile();
        }
        else
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag {tag} not found in manifest");
        }
    }
}
