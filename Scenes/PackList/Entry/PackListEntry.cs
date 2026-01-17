using System;
using Godot;
using Packkit.Manifest;
using Packkit.PackManagement;

namespace Packkit.Ui;

public partial class PackListEntry : Control
{
    [Export]
    public Label PackNameLabel;
    public Guid PackId;

    public void Initialize()
    {
        PackNameLabel.Text = PackManager.Packs[PackId].Header.Name;
    }

    private void _on_delete_button_pressed()
    {
        PackManager.DeletePack(PackId);
        QueueFree();
    }

    private void _on_open_folder_button_pressed()
    {
        OS.ShellOpen(PackManager.GetPackFolderPath(PackId));
    }
}
