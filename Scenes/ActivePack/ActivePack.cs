using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Packkit.Globals;
using Packkit.Ui.Nodes;

namespace Packkit.Ui;

public partial class ActivePack : VBoxContainer
{
    [Export]
    public Pack pack;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += OnActivePackChanged;
    }

    public void OnActivePackChanged()
    {
        pack.PackId = PackManager.ActivePack.Item1;
        pack.PackNameLabel.Text = PackManager.ActivePack.Item2.Header.Name;
    }

    private void _on_delete_button_pressed() => pack.Delete();

    private void _on_open_folder_button_pressed() => pack.OpenPackFolder();

    private void _on_scan_button_pressed() => pack.Scan();
}
