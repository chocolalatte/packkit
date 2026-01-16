using System;
using Godot;
using Packkit.PackManagement;

public partial class PackList : Control
{
    [Export]
    public PackedScene PackListEntryScene;

    [Export]
    public VBoxContainer PackListEntryContainer;

    public override void _Ready()
    {
        PackManager.Initialize();
        foreach (var pack in PackManager.Packs.Values)
        {
            PackListEntry entry = (PackListEntry)PackListEntryScene.Instantiate();
            PackListEntryContainer.AddChild(entry);

            entry.PackName.Text = pack.Header.Name;
        }
    }
}
