using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
    private ModList modList;

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
        foreach (var child in TagEntryContainer.GetChildren())
        {
            child.QueueFree();
        }
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
        using Tagger tagger = new(PackManager.ActivePack?.Item2);
        foreach (var tagEntry in TagEntryContainer.GetChildren())
        {
            if (tagEntry is SimpleTagEntry simpleTagEntry)
            {
                if (simpleTagEntry.IsSelected)
                {
                    tagger.SimpleTags.Add(simpleTagEntry.Tag);
                }
                else
                {
                    GD.Print($"[TAGLIST] [INFO] Unselected tag: \"{simpleTagEntry.Tag.Name}\"");
                }
            }
            else if (tagEntry is ValueTagEntry valueTagEntry)
            {
                if (valueTagEntry.GetValue() != null)
                {
                    ValueTagInstance valueTagInstance = new(
                        valueTagEntry.Tag,
                        valueTagEntry.GetValue()
                    );
                    tagger.ValueTags.Add(valueTagInstance);
                }
                else
                {
                    GD.Print(
                        $"[TAGLIST] [WARN] Value tag value is null: \"{valueTagEntry.Tag.Name}\""
                    );
                }
            }
            else if (tagEntry is EnumTagEntry enumTagEntry)
            {
                if (enumTagEntry.GetValue() != null)
                {
                    EnumTagInstance enumTagInstance = new(
                        enumTagEntry.Tag,
                        enumTagEntry.GetValue()
                    );
                    tagger.EnumTags.Add(enumTagInstance);
                }
                else
                {
                    GD.Print(
                        $"[TAGLIST] [WARN] Enum tag value is null: \"{enumTagEntry.Tag.Name}\""
                    );
                }
            }
        }
        tagger.AddAllTags(modList.SelectedMod.Entry);
        tagger.Save();
    }
}
