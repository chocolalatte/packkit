using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Packkit.Globals;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Godot;

public partial class TagList : Control
{
    [Export]
    private PackedScene SimpleTagEntryScene;

    [Export]
    private VBoxContainer TagEntryContainer;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += UpdateTagList;
    }

    public void UpdateTagList()
    {
        foreach (var tag in PackManager.ActivePack?.Item2.Customization.Tags.SimpleTags)
        {
            AddSimpleTag(tag);
        }
    }

    private void AddSimpleTag(SimpleTagDefinition tag)
    {
        SimpleTagEntry simpleTagEntry = (SimpleTagEntry)SimpleTagEntryScene.Instantiate();
        simpleTagEntry.Initialize(tag);
        TagEntryContainer.AddChild(simpleTagEntry);
    }
}
