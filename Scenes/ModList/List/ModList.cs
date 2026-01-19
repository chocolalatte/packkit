using System;
using System.Collections.Generic;
using Godot;
using Packkit.Globals;
using Packkit.Manifest;

namespace Packkit.Godot;

public partial class ModList : Control
{
    [Export]
    public VBoxContainer ModEntryContainer;

    [Export]
    public PackedScene ModListEntryScene;
    public Dictionary<string, ModEntry> mods;

    public override void _Ready() { }

    public void PopulateModList()
    {
        mods = PackManager.ActivePack.Item2.Mods;
        foreach (ModEntry modEntry in mods.Values)
        {
            AddEntry(modEntry);
        }
    }

    public void _on_visibility_changed()
    {
        if (Visible)
        {
            PopulateModList();
        }
    }

    private void AddEntry(ModEntry modEntry)
    {
        ModListEntry modListEntry = (ModListEntry)ModListEntryScene.Instantiate();
        ModEntryContainer.AddChild(modListEntry);
        modListEntry.Initialize(modEntry);
    }
}
