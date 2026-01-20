using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Packkit.Globals;
using Packkit.Manifest;
using Packkit.Tags;

namespace Packkit.Godot;

public partial class ModList : Control
{
    [Export]
    private Popup TagListPopup;

    [Export]
    public VBoxContainer ModEntryContainer;

    [Export]
    public PackedScene ModListEntryScene;

    public ModRef SelectedMod;
    public static Dictionary<string, ModEntry> Mods
    {
        get { return PackManager.ActivePack?.Item2.Mods; }
    }
    public static IEnumerable<ModRef> ModRefs =>
        Mods?.Select(kvp => new ModRef(kvp.Key, kvp.Value));

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += UpdateModList;
    }

    public void UpdateModList()
    {
        PopulateModList();
    }

    private void PopulateModList()
    {
        GD.Print($"[MODLIST] [INFO] Populating mod list");
        foreach (var child in ModEntryContainer.GetChildren())
        {
            child.QueueFree();
        }

        if (!(Mods?.Count > 0))
            GD.Print($"[MODLIST] [WARN] No mods found");
        try
        {
            foreach (ModRef modEntry in ModRefs)
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

    private void AddEntry(ModRef modRef)
    {
        ModListEntry modListEntry = (ModListEntry)ModListEntryScene.Instantiate();
        ModEntryContainer.AddChild(modListEntry);
        modListEntry.EditTagsButtonPressed += ShowTagList;
        modListEntry.Initialize(modRef);
    }

    private void ShowTagList(ModListEntry modListEntry)
    {
        TagListPopup.Visible = true;
        SelectedMod = modListEntry.Mod;
    }

    private void _on_toggle_selection_button_toggled(bool toggled)
    {
        if (toggled)
        {
            foreach (ModListEntry child in ModEntryContainer.GetChildren().OfType<ModListEntry>())
            {
                child.CheckBox.Visible = true;
            }
        }
        else
        {
            foreach (ModListEntry child in ModEntryContainer.GetChildren().OfType<ModListEntry>())
            {
                child.CheckBox.Visible = false;
                child.CheckBox.ButtonPressed = false;
            }
        }
    }

    private void _on_disable_mods_button_pressed()
    {
        Guid packId = PackManager.ActivePack.Item1;
        List<string> modHashes = [];
        foreach (ModListEntry child in ModEntryContainer.GetChildren().OfType<ModListEntry>())
        {
            if (child.CheckBox.ButtonPressed)
            {
                modHashes.Add(child.Mod.FileHash);
            }
        }

        PackManager.ToggleModsEnabled(packId, modHashes);
    }
}
