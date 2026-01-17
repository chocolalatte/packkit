using System;
using System.Runtime.CompilerServices;
using Godot;
using Packkit.Globals;

namespace Packkit.Ui;

public partial class PackList : Control
{
    [Export]
    public VBoxContainer PackListEntryContainer;

    [Export]
    public AspectRatioContainer NewPackMenu;

    [Export]
    public TextEdit PackNameTextEdit;

    [Export]
    public PackedScene PackListEntryScene;

    public string NewPackName;

    public override void _Ready()
    {
        foreach (var pack in PackManager.Packs)
        {
            AddEntry(pack.Key);
        }
    }

    private void AddEntry(Guid packId)
    {
        PackListEntry entry = (PackListEntry)PackListEntryScene.Instantiate();

        entry.pack.PackId = packId;
        entry.pack.Initialize();

        PackListEntryContainer.AddChild(entry);
    }

    private void _on_create_pack_menu_button_pressed()
    {
        NewPackMenu.Visible = true;
    }

    private void _on_back_button_pressed()
    {
        NewPackMenu.Visible = false;
    }

    private void _on_pack_name_text_edit_text_changed()
    {
        NewPackName = PackNameTextEdit.Text;
    }

    private void _on_create_pack_button_pressed()
    {
        Guid packId = PackManager.CreatePack(NewPackName, "TestAuthor");

        AddEntry(packId);
    }
}
