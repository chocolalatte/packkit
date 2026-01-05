using System.Collections.Generic;
using System.Net.NetworkInformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Packkit.Core;

public partial class ModEntry : ObservableObject
{
    public string? ModId { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? File { get; set; } = null;
    public string? Version { get; set; } = null;
    public ModLoader Loader { get; set; } = ModLoader.unknown;
    public ModSide Side { get; set; } = ModSide.unknown;

    [ObservableProperty]
    private ModImportance importance = ModImportance.unknown;

    [ObservableProperty]
    private ModImpact impact = ModImpact.unknown;

    public List<string> Notes { get; set; } = [];
    public int Priority { get; set; } = 0;

    public List<string> Requires { get; set; } = [];
    public List<string> Recommends { get; set; } = [];
    public ModTags Tags { get; set; } = new ModTags();
}

public class ModTags
{
    public List<string> SimpleTags { get; set; } = [];
    public Dictionary<string, string> ValueTags { get; set; } = [];
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

public enum ModImportance
{
    unknown,
    required,
    recommended,
    optional,
}

public enum ModImpact
{
    unknown,
    light,
    heavy,
    performance,
}
