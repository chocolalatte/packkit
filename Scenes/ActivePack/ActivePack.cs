using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Packkit.Globals;
using Packkit.Godot.Nodes;

namespace Packkit.Godot;

public partial class ActivePack : VBoxContainer
{
    [Export]
    public Pack pack;

    private void _on_delete_button_pressed() => pack.Delete();

    private void _on_open_folder_button_pressed() => pack.OpenPackFolder();

    private void _on_scan_button_pressed() => pack.Scan();
}
