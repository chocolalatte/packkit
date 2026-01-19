using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Packkit.Manifest;

namespace Packkit.Godot;

public partial class ModListEntry : HBoxContainer
{
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
}
