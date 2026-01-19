using System;
using Godot;
using Packkit.Globals;
using Packkit.Godot.Nodes;

namespace Packkit.Godot;

public partial class Home : MarginContainer
{
    [Export]
    public PackedScene ActivePackScene;
    private ActivePack ActivePackNode;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.ActivePackChanged += OnActivePackChanged;
    }

    public void OnActivePackChanged()
    {
        if (IsInstanceValid(ActivePackNode))
        {
            ActivePackNode.QueueFree();
            ActivePackNode = null;
        }

        ActivePackNode = ActivePackScene.Instantiate<ActivePack>();
        AddChild(ActivePackNode);
        Pack pack = ActivePackNode.GetNode<Pack>("Pack");

        pack.PackId = PackManager.ActivePack.Item1;
        pack.PackNameLabel.Text = PackManager.ActivePack.Item2.Header.Name;
    }
}
