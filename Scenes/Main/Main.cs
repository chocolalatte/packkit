using System;
using Godot;
using Packkit.PackManagement;

namespace Packkit.Ui;

public partial class Main : Control
{
    [Export]
    public PackList PackList;
    private string packName;

    private void _on_button_pressed()
    {
        if (packName == null)
        {
            return;
        }

        PackManager.CreatePack(packName, "TestAuthor");
    }
}
