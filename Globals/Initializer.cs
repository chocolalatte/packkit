using System;
using Godot;
using Packkit.PackManagement;

public partial class Initializer : Node
{
    public override void _Ready()
    {
        PackManager.Initialize();
    }
}
