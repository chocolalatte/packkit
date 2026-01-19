using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Packkit.Manifest;

namespace Packkit.Godot;

public partial class ModListEntry : HBoxContainer
{
    [Export]
    public Label NameLabel;
    public ModEntry Mod;

    public void Initialize(ModEntry mod)
    {
        Mod = mod;
        NameLabel.Text = Mod.Name;
    }
}
