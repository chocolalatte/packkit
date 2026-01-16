using System;
using Godot;
using Packkit.PackManagement;

namespace Packkit.Ui;

public partial class Main : Control
{
    private void _on_button_pressed()
    {
        PackManager.CreatePack("TestPack", "TestAuthor");
    }
}
