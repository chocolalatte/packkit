using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Packkit.Manifest;

namespace Packkit.Godot;

public partial class ModListEntry : HBoxContainer
{
    [Signal]
    public delegate void EditTagsButtonPressedEventHandler(ModListEntry modListEntry);

    [Export]
    public CheckBox CheckBox;

    [Export]
    public Label NameLabel;

    public ModRef Mod;

    public void Initialize(ModRef modRef)
    {
        Mod = modRef;
        NameLabel.Text = Mod.Entry.Name;
    }

    private void _on_edit_tags_button_pressed() => EmitSignal(nameof(EditTagsButtonPressed), this);
}
