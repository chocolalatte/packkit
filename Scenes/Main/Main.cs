using System;
using Godot;
using Packkit.PackManagement;

namespace Packkit.Ui;

public partial class Main : Control
{
    [Export]
    public TextEdit textEdit;
    private string packName;

    private void _on_button_pressed()
    {
        if (packName == null)
        {
            return;
        }

        PackManager.CreatePack(packName, "TestAuthor");
    }

    private void _on_text_edit_text_changed()
    {
        packName = textEdit.Text;
    }
}
