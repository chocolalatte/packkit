using System;
using Godot;
using Packkit.Core;
using Packkit.Globals;

namespace Packkit.Ui.Nodes;

[GlobalClass]
public partial class Pack : Control
{
    [Signal]
    public delegate void SetActivePackEventHandler();

    [Export]
    public Label PackNameLabel;
    public Guid PackId;

    public void Initialize()
    {
        PackNameLabel.Text = PackManager.Packs[PackId].Header.Name;
    }

    public void Delete()
    {
        PackManager.DeletePack(PackId);
        GetParent().QueueFree();
    }

    public void OpenPackFolder()
    {
        OS.ShellOpen(PackManager.GetPackFolderPath(PackId));
    }

    public void Scan()
    {
        Scanner.ScanFiles(PackManager.GetPackFolderPath(PackId));
    }

    public void SetActive()
    {
        PackManager.PackManagerInstance.SetActivePack(PackId);
    }
}
