using System;
using Godot;
using Packkit.Globals;
using Packkit.Ui.Nodes;

namespace Packkit.Ui;

public partial class Home : MarginContainer
{
    [Export]
    public PackedScene ActivePackScene;

    private VBoxContainer ActivePackNode;
    private Pack pack;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += OnActivePackChanged;
    }

    public void OnActivePackChanged()
    {
        if (ActivePackNode == null)
        {
            ActivePackNode = (VBoxContainer)ActivePackScene.Instantiate();
            AddChild(ActivePackNode);
            pack = (Pack)ActivePackNode.GetNode("Pack");
        }

        pack.PackId = PackManager.ActivePack.Item1;
        pack.PackNameLabel.Text = PackManager.ActivePack.Item2.Header.Name;
    }
}
