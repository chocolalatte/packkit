using System;
using Godot;
using Packkit.Globals;
using Packkit.Ui.Nodes;

namespace Packkit.Ui;

public partial class Home : MarginContainer
{
    [Export]
    public PackedScene ActivePackScene;
    private Pack pack;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += OnActivePackChanged;
    }

    public void OnActivePackChanged()
    {
        VBoxContainer activePackNode = (VBoxContainer)ActivePackScene.Instantiate();
        AddChild(activePackNode);
        pack = (Pack)activePackNode.GetNode("Pack");

        pack.PackId = PackManager.ActivePack.Item1;
        pack.PackNameLabel.Text = PackManager.ActivePack.Item2.Header.Name;
    }
}
