using System;
using Godot;
using Packkit.Core;
using Packkit.Globals;
using Packkit.Manifest;
using Packkit.Ui.Nodes;

namespace Packkit.Ui;

public partial class PackListEntry : HBoxContainer
{
	[Export]
	public Pack pack;

	private void _on_delete_button_pressed() => pack.Delete();

	private void _on_open_folder_button_pressed() => pack.OpenPackFolder();

	private void _on_scan_button_pressed() => pack.Scan();

	private void _on_set_active_button_pressed() => pack.SetActive();
}
