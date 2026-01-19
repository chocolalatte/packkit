using System;
using Godot;
using Packkit.Core;
using Packkit.Globals;
using static Packkit.Core.Scanner;

namespace Packkit.Godot.Nodes;

[GlobalClass]
public partial class Pack : Control
{
    [Signal]
    public delegate void SetActivePackEventHandler();

    [Export]
    public Label PackNameLabel;
    public Guid PackId;

    public override void _Ready()
    {
        PackManager.PackManagerInstance.PackDeleted += _on_pack_deleted;
    }

    public void Initialize()
    {
        PackNameLabel.Text = PackManager.Packs[PackId].Header.Name;
    }

    public void Delete()
    {
        PackManager.PackManagerInstance.DeletePack(PackId);
    }

    private void _on_pack_deleted(string packIdString)
    {
        if (PackId.ToString() == packIdString)
        {
            GetParent().QueueFree();
        }
    }

    public void OpenPackFolder()
    {
        OS.ShellOpen(PackManager.GetPackFolderPath(PackId));
    }

    public void Scan()
    {
        string folderPath = PackManager.GetPackFolderPath(PackId);
        using Scanner scanner = new(folderPath);
    }

    public void SetActive()
    {
        PackManager.PackManagerInstance.SetActivePack(PackId);
    }
}
