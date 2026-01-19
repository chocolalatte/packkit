using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Godot;
using Packkit.Manifest;
using Tomlyn;

namespace Packkit.Globals;

[GlobalClass]
public partial class PackManager : Node
{
    [Signal]
    public delegate void PackDeletedEventHandler(string packIdString);

    [Signal]
    public delegate void ActivePackChangedEventHandler();

    public static PackManager PackManagerInstance { get; private set; }
    public static Tuple<Guid, PackManifest> ActivePack { get; private set; }
    public static Dictionary<Guid, PackManifest> Packs { get; private set; } = [];
    private static readonly string packsFolder = Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        "Pakkit",
        "packs"
    );

    public void SetActivePack(Guid packId)
    {
        var NewActivePack = Tuple.Create(packId, Packs[packId]);
        // Check if pack is already active
        if (ActivePack?.Item1 == NewActivePack.Item1)
        {
            GD.Print(
                $"[GLOBALS:PACKMANAGER] [INFO] Active pack already set to \"{NewActivePack.Item2.Header.Name}\""
            );
            return;
        }

        GD.Print(
            $"[GLOBALS:PACKMANAGER] [INFO] Active pack changed to \"{NewActivePack.Item2.Header.Name}\""
        );

        // Set new active pack
        ActivePack = NewActivePack;

        EmitSignal(nameof(ActivePackChanged));
    }

    public override void _Ready()
    {
        PackManagerInstance = this;
        LoadAllPacks();
    }

    private static void LoadAllPacks()
    {
        Console.WriteLine("[GLOBALS:PACKMANAGER] [INFO] Loading all packs");

        var packDirectories = Directory.EnumerateDirectories(packsFolder);

        // Variables for keeping track of progress
        int totalPackCount = packDirectories.Count();
        int failedScanCount = 0;
        int packsScannedCount = 0;

        int scannedPackCount() => packsScannedCount + failedScanCount;

        Packs.Clear();

        foreach (var packDirectory in packDirectories)
        {
            var packDirectoryName = Path.GetFileName(packDirectory);
            var manifestPath = Path.Combine(packDirectory, "manifest.toml");

            try
            {
                PackManifest manifest = PackManifest.LoadExisting(manifestPath);
                Packs.Add(Guid.Parse(manifest.Header.Id!), manifest);

                // Log progress
                packsScannedCount++;
                GD.Print(
                    $"[GLOBALS:PACKMANAGER] [PROGRESS] [{scannedPackCount()}/{totalPackCount}] Successfully loaded pack \"{packDirectoryName}\""
                );
            }
            catch (Exception exception)
            {
                // Log progress and error
                failedScanCount++;
                GD.Print(
                    $"[GLOBALS:PACKMANAGER] [PROGRESS] [{scannedPackCount()}/{totalPackCount}] Failed to load pack \"{packDirectoryName}\" reason: \"{exception.Message}\""
                );
                continue;
            }
        }

        // Log summary
        GD.Print(
            $"[GLOBALS:PACKMANAGER] [INFO] {packsScannedCount} out of {totalPackCount} packs loaded successfully: {failedScanCount} packs failed"
        );
    }

    public static string GetPackFolderPath(Guid? guid)
    {
        // Use active pack if no name is provided
        if (guid == null)
        {
            // Throw error if no active pack is selected
            if (ActivePack == null)
            {
                throw new Exception($"[GLOBALS:PACKMANAGER] [ERROR-003] No active pack selected");
            }

            guid = ActivePack.Item1;
        }

        string name = Packs[guid.Value].Header.Name;
        string packPath = Path.Combine(packsFolder, name!);

        // Check if the pack exists
        if (!Directory.Exists(packPath))
        {
            throw new DirectoryNotFoundException(
                $"[GLOBALS:PACKMANAGER] [ERROR-001] PackManifest for pack \"{name}\" with guid \"{guid}\" not found at path {packPath}"
            );
        }

        return packPath;
    }

    public static Guid CreatePack(string packName, string packAuthor)
    {
        GD.Print($"[GLOBALS:PACKMANAGER] [INFO] Creating pack \"{packName}\"");
        string packPath = Path.Combine(packsFolder, packName);

        // Check if pack already exists
        if (Directory.Exists(packPath))
        {
            throw new Exception(
                $"[GLOBALS:PACKMANAGER] [ERROR-002] Pack \"{packName}\" already exists"
            );
        }

        Directory.CreateDirectory(packPath);
        Directory.CreateDirectory(packPath + "/mods");
        Directory.CreateDirectory(packPath + "/disabled");

        PackManifest manifest = Toml.ToModel<PackManifest>(Defaults.BaseManifest);

        // Set pack info
        manifest.Header.Name = packName;
        manifest.Header.Author = packAuthor;

        string packSlug = $"{packAuthor}.{packName}".ToLower().Replace(" ", "-");
        manifest.Header.Slug = packSlug;

        Guid guid = Guid.NewGuid();
        manifest.Header.Id = guid.ToString();

        // Save manifest and add to packs dictionary
        Packs.Add(guid, manifest);
        manifest.SaveToFile(packPath + "/manifest.toml");

        GD.Print(
            $"[GLOBALS:PACKMANAGER] [INFO] Pack \"{packName}\" created successfully with slug {packSlug}"
        );

        return guid;
    }

    public void DeletePack(Guid packId)
    {
        string packName = Packs[packId].Header.Name;
        string packSlug = Packs[packId].Header.Slug;
        GD.Print($"[GLOBALS:PACKMANAGER] [INFO] Deleting pack \"{packName}\"");

        try
        {
            string packPath = GetPackFolderPath(packId);
            Directory.Delete(packPath, true);
            Packs.Remove(packId);
            EmitSignal(nameof(PackDeleted), packId.ToString());

            GD.Print(
                $"[GLOBALS:PACKMANAGER] [INFO] Pack \"{packName}\" with slug \"{packSlug}\" deleted successfully"
            );
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GLOBALS:PACKMANAGER] [ERROR-004] Failed to delete pack: {ex.Message}");
        }
    }

    public static void ToggleModsEnabled(Guid packId, List<string> modHashes)
    {
        foreach (string modHash in modHashes)
        {
            if (!Packs[packId].Mods.TryGetValue(modHash, out ModEntry modEntry))
            {
                GD.Print($"[GLOBALS:PACKMANAGER] [ERROR-005] Mod \"{modHash}\" not found in pack");
                continue;
            }

            string packPath = GetPackFolderPath(packId);
            string enabledModPath = Path.Combine(packPath, "mods", modEntry.File);
            string disabledModPath = Path.Combine(packPath, "disabled", modEntry.File);

            string sourcePath;
            string targetPath;

            if (File.Exists(enabledModPath))
            {
                sourcePath = enabledModPath;
                targetPath = disabledModPath;
            }
            else if (File.Exists(disabledModPath))
            {
                sourcePath = disabledModPath;
                targetPath = enabledModPath;
            }
            else
            {
                GD.Print(
                    $"[GLOBALS:PACKMANAGER] [ERROR-006] Mod \"{modHash}\" not found in pack \"{Packs[packId].Header.Name}\""
                );
                continue;
            }

            try
            {
                File.Move(sourcePath, targetPath);
            }
            catch (Exception ex)
            {
                GD.Print(
                    $"[GLOBALS:PACKMANAGER] [ERROR-007] Failed to move mod \"{modEntry.Name}\": \n{ex}"
                );
            }
        }
    }
}
