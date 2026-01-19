using System;
using Godot;
using Packkit.Globals;

namespace Packkit.Godot;

public partial class Main : Control
{
    [Export]
    public PackList PackList;
    private string packName;
}
