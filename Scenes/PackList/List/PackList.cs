using System;
using System.Runtime.CompilerServices;
using Godot;
using Packkit.PackManagement;

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
        PackManager.Initialize();
        foreach (var pack in PackManager.Packs.Values)
        {
            AddEntry(pack.Header.Name);
        }
    }

    private void AddEntry(string packName)
    {
        PackListEntry entry = (PackListEntry)PackListEntryScene.Instantiate();
        PackListEntryContainer.AddChild(entry);

        entry.PackName.Text = packName;
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
        PackManager.CreatePack(NewPackName, "TestAuthor");

        AddEntry(NewPackName);
    }
}
