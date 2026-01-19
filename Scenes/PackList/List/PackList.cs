using System;
using System.Runtime.CompilerServices;
using Godot;
using Packkit.Globals;
using Packkit.Godot.Nodes;

namespace Packkit.Godot;

public partial class PackList : Control
{
    [Export]
    public VBoxContainer PackListEntryContainer;

    [Export]
    public AspectRatioContainer NewPackMenu;

    [Export]
    public LineEdit PackNameLineEdit;

    [Export]
    public PackedScene PackListEntryScene;

    public void PopulatePackList()
    {
        GD.Print($"[PACKLIST] [INFO] Populating pack list");
        foreach (var pack in PackManager.Packs)
        {
            AddEntry(pack.Key);
        }
    }

    private void _on_visibility_changed()
    {
        if (
            Visible
            && PackManager.Packs.Count > 0
            && PackListEntryContainer.GetChildren().Count == 0
        )
        {
            PopulatePackList();
        }
    }

    private void AddEntry(Guid packId)
    {
        PackListEntry entry = (PackListEntry)PackListEntryScene.Instantiate();

        entry.pack.PackId = packId;
        entry.pack.Initialize();

        PackListEntryContainer.AddChild(entry);
    }

    private void CreatePack()
    {
        Guid packId = PackManager.CreatePack(PackNameLineEdit.Text, "TestAuthor");

        AddEntry(packId);
        PackNameLineEdit.Text = "";
    }

    private void _on_create_pack_menu_button_pressed()
    {
        NewPackMenu.Visible = true;
    }

    private void _on_back_button_pressed()
    {
        NewPackMenu.Visible = false;
    }

    private void _on_create_pack_button_pressed()
    {
        CreatePack();
    }

    private void _on_pack_name_line_edit_text_submitted(string newText)
    {
        CreatePack();
    }
}
