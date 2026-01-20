using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Packkit.Globals;
using Packkit.Godot.Nodes;
using Packkit.Godot.TagEntry;
using Packkit.Tags;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Godot;

public partial class TagList : Popup
{
    [Export]
    private PackedScene SimpleTagEntryScene;

    [Export]
    private PackedScene ValueTagEntryScene;

    [Export]
    private PackedScene EnumTagEntryScene;

    [Export]
    private VBoxContainer TagEntryContainer;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += UpdateTagList;
    }

    public void UpdateTagList()
    {
        GD.Print($"[TAGLIST] [INFO] Updating tag list");
        foreach (var tag in PackManager.ActivePack?.Item2.Customization.Tags.SimpleTags)
        {
            AddSimpleTagEntry(tag);
        }

        foreach (var tag in PackManager.ActivePack?.Item2.Customization.Tags.ValueTags)
        {
            AddValueTagEntry(tag);
        }

        foreach (var tag in PackManager.ActivePack?.Item2.Customization.Tags.EnumTags)
        {
            AddEnumTagEntry(tag);
        }
    }

    private void AddSimpleTagEntry(SimpleTagDefinition tag)
    {
        SimpleTagEntry simpleTagEntry = (SimpleTagEntry)SimpleTagEntryScene.Instantiate();
        simpleTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(simpleTagEntry);
    }

    private void AddValueTagEntry(ValueTagDefinition tag)
    {
        ValueTagEntry valueTagEntry = (ValueTagEntry)ValueTagEntryScene.Instantiate();
        valueTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(valueTagEntry);
    }

    private void AddEnumTagEntry(EnumTagDefinition tag)
    {
        EnumTagEntry enumTagEntry = (EnumTagEntry)EnumTagEntryScene.Instantiate();
        enumTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(enumTagEntry);
    }

    private void _on_apply_tags_button_pressed()
    {
        GetSelectedTags();
    }

    private void GetSelectedTags()
    {
        foreach (var tagEntry in TagEntryContainer.GetChildren())
        {
            if (tagEntry is SimpleTagEntry simpleTagEntry)
            {
                if (simpleTagEntry.IsSelected)
                {
                    GD.Print($"[TAGLIST] [INFO] Selected tag: \"{simpleTagEntry.Tag.Name}\"");
                }
            }
            else if (tagEntry is ValueTagEntry valueTagEntry)
            {
                if (valueTagEntry.GetValue() != null)
                {
                    GD.Print(
                        $"[TAGLIST] [INFO] Selected tag: \"{valueTagEntry.Tag.Name}\" with value \"{valueTagEntry.GetValue()}\""
                    );
                }
            }
            else if (tagEntry is EnumTagEntry enumTagEntry)
            {
                if (enumTagEntry.GetValue() != null)
                {
                    GD.Print(
                        $"[TAGLIST] [INFO] Selected tag: \"{enumTagEntry.Tag.Name}\" with value \"{enumTagEntry.GetValue()}\""
                    );
                }
            }
        }
    }
}
