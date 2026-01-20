using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Godot;
using Packkit.Globals;
using Packkit.Godot.Nodes;
using Packkit.Manifest;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Tags;

public sealed class Tagger : IDisposable
{
    public Tagger(PackManifest manifest)
    {
        GD.Print($"[MANIFEST:TAGGER] [INFO] Tagger created");
        Manifest = manifest;
    }

    private readonly PackManifest Manifest;
    private List<SimpleTagDefinition> SimpleTags;
    private List<ValueTagDefinition> ValueTags;
    private List<EnumTagDefinition> EnumTags;

    public void AddSimpleTags(ModEntry modEntry, List<SimpleTagDefinition> tags)
    {
        SimpleTags = Manifest.Customization.Tags.SimpleTags;
        foreach (SimpleTagDefinition tag in tags)
        {
            AddSimpleTag(modEntry, tag);
        }
    }

    private void AddSimpleTag(ModEntry modEntry, SimpleTagDefinition tag)
    {
        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (!SimpleTags.Any(t => t.Name == tag.Name))
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [ERROR-001] SimpleTag \"{tag.Name}\" not found in manifest"
            );
            return;
        }
        // Check if tag is already applied to mod
        // Return and log warning if it is
        if (modEntry.Tags.Simple.Contains(tag.Name))
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [WARN] SimpleTag \"{tag.Name}\" already exists in \"{modEntry.Name}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Simple.Add(tag.Name);

        GD.Print($"[MANIFEST:TAGGER] [INFO] Added SimpleTag \"{tag}\" to \"{modEntry.Name}\"");
    }

    public void AddValueTags(ModEntry modEntry, List<ValueTagInstance> tags)
    {
        ValueTags = Manifest.Customization.Tags.ValueTags;
        foreach (ValueTagInstance tag in tags)
        {
            AddValueTag(modEntry, tag);
        }
    }

    public void AddValueTag(ModEntry modEntry, ValueTagInstance tagInstance)
    {
        var tag = tagInstance.Definition;
        var value = tagInstance.Value;
        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (!ValueTags.Any(t => t.Name == tag.Name))
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [ERROR-002] ValueTag \"{tag.Name}\" not found in manifest"
            );
            return;
        }

        var normalizedValue = NormalizeValueTagObject(
            ValueTags.First(t => t.Name == tag.Name),
            value
        );
        // Check if tag already has same value
        // Return and log warning if it does
        if (
            modEntry.Tags.Value.TryGetValue(tag.Name, out object existingValue)
            && existingValue.Equals(normalizedValue)
        )
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [WARN] ValueTag \"{tag.Name}\" already exists in \"{modEntry.Name}\" with value \"{normalizedValue}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Value[tag.Name] = normalizedValue;
        GD.Print(
            $"[MANIFEST:TAGGER] [INFO] Added ValueTag \"{tag.Name}\" with value of \"{normalizedValue}\" to \"{modEntry.Name}\""
        );
    }

    // Convert object to a TOML supported type
    private static object NormalizeValueTagObject(ValueTagDefinition definition, object value)
    {
        return definition.Type switch
        {
            ValueTagType.Integer => Int64.Parse(value.ToString()),
            ValueTagType.Float => float.Parse(value.ToString()),
            ValueTagType.Boolean => bool.Parse(value.ToString()),
            ValueTagType.String => value,
            _ => value,
        };
    }

    public void AddEnumTags(ModEntry modEntry, List<EnumTagInstance> tags)
    {
        EnumTags = Manifest.Customization.Tags.EnumTags;
        foreach (EnumTagInstance tag in tags)
        {
            AddEnumTag(modEntry, tag);
        }
    }

    public void AddEnumTag(ModEntry modEntry, EnumTagInstance tagInstance)
    {
        var tag = tagInstance.Definition;
        var value = tagInstance.Value;
        var enumTag = EnumTags.FirstOrDefault(t => t.Name == tag.Name);

        // Check if tag exists in manifest's tag definitions
        // Return and log error if it doesn't
        if (enumTag == null)
        {
            GD.Print($"[MANIFEST:TAGGER] [ERROR-003] EnumTag \"{tag.Name}\" not found in manifest");
            return;
        }
        // Check if tags options contains value
        // Return and log error if it doesn't
        else if (!enumTag.Options.Contains(value))
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [ERROR-004] EnumTag \"{tag.Name}\" does not contain possible value \"{value}\""
            );
            return;
        }
        // Run if manifest contains definition for tag and contains value
        // Check if tag already has same value
        if (
            modEntry.Tags.Enum.TryGetValue(tag.Name, out string existingValue)
            && existingValue == value
        )
        {
            GD.Print(
                $"[MANIFEST:TAGGER] [WARN] EnumTag \"{tag.Name}\" already exists in \"{modEntry.Name}\" with value \"{value}\""
            );
            return;
        }
        // Core logic

        modEntry.Tags.Enum[tag.Name] = value;
        GD.Print(
            $"[MANIFEST:TAGGER] [INFO] Added EnumTag \"{tag.Name}\" with value of \"{value}\" to \"{modEntry.Name}\""
        );
    }

    public void Dispose()
    {
        Manifest.SaveToFile(PackManager.GetPackFolderPath(Guid.Parse(Manifest.Header.Id)));
        GD.Print($"[MANIFEST:TAGGER] [INFO] Tagger disposed");
    }
}
