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
    public static Dictionary<string, ModEntry> Mods
    {
        get { return PackManager.ActivePack?.Item2.Mods; }
    }

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += PopulateModList;
    }

    public void PopulateModList()
    {
        GD.Print($"[MODLIST] [INFO] Populating mod list");
        foreach (var child in ModEntryContainer.GetChildren())
        {
            child.QueueFree();
        }

        if (Mods?.Count > 0)
        {
            try
            {
                foreach (ModEntry modEntry in Mods.Values)
                {
                    AddEntry(modEntry);
                }
                GD.Print($"[MODLIST] [INFO] Successfully populated mod list");
            }
            catch (Exception ex)
            {
                GD.Print($"[MODLIST] [ERROR] Failed to populate mod list: {ex.Message}");
            }
        }
        else
        {
            GD.Print($"[MODLIST] [WARN] No mods found");
        }
    }

    private void AddEntry(ModEntry modEntry)
    {
        ModListEntry modListEntry = (ModListEntry)ModListEntryScene.Instantiate();
        ModEntryContainer.AddChild(modListEntry);
        modListEntry.Initialize(modEntry);
    }
}
