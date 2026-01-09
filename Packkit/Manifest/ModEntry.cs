using System.Collections.Generic;
using System.Net.NetworkInformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Packkit.Manifest;

public partial class ModEntry : ObservableObject
{
    public string? ModId { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? File { get; set; } = null;
    public string? Version { get; set; } = null;
    public ModLoader Loader { get; set; } = ModLoader.unknown;
    public ModSide Side { get; set; } = ModSide.unknown;

    public List<string> Notes { get; set; } = [];

    public List<string> Requires { get; set; } = [];
    public List<string> Recommends { get; set; } = [];

    public ModTags Tags { get; set; } = new();
}

public class ModTags
{
    public List<string> Simple { get; set; } = [];
    public Dictionary<string, object> Value { get; set; } = [];
    public Dictionary<string, string> Enum { get; set; } = [];
}

public enum ModLoader
{
    unknown,
    forge,
    neoforge,
    fabric,
}

public enum ModSide
{
    unknown,
    client,
    server,
    both,
}
