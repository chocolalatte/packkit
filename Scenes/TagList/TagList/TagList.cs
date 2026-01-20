using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Packkit.Globals;
using Packkit.Godot.TagEntry;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Godot;

public partial class TagList : Popup
{
    [Export]
    private PackedScene SimpleTagEntryScene;

    [Export]
    private PackedScene ValueTagEntryScene;

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
            AddSimpleTag(tag);
        }

        foreach (var tag in PackManager.ActivePack?.Item2.Customization.Tags.ValueTags)
        {
            AddValueTag(tag);
        }
    }

    private void AddSimpleTag(SimpleTagDefinition tag)
    {
        SimpleTagEntry simpleTagEntry = (SimpleTagEntry)SimpleTagEntryScene.Instantiate();
        simpleTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(simpleTagEntry);
    }

    private void AddValueTag(ValueTagDefinition tag)
    {
        ValueTagEntry valueTagEntry = (ValueTagEntry)ValueTagEntryScene.Instantiate();
        valueTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(valueTagEntry);
    }
}
