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

    private void _on_tab_bar_tab_changed(int index)
    {
        switch (index)
        {
            case 0:
                PackList.Visible = false;
                break;
            case 1:
                PackList.Visible = true;
                break;
        }
    }
}
