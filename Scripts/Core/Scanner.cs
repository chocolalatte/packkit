using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
using Godot;
using Microsoft.VisualBasic;
using Packkit.Manifest;
using Tomlyn;
using Tomlyn.Model;
using static Packkit.Core.Utils;

namespace Packkit.Core;

public sealed class Scanner : IDisposable
{
    // Constructor
    public Scanner(string folderPath)
    {
        GD.Print($"[SCANNER] [INFO] Scanner initialized");
        ScanFiles(folderPath);
    }

    private PackManifest manifest;
    private IEnumerable<string> modFilePaths;

    // Variables for keeping track of progress
    private int totalFileCount = 0;
    private int failedScanCount = 0;
    private int modsScannedCount = 0;

    private int ScannedFileCount() => modsScannedCount + failedScanCount;

    // Some files may have both a mods.toml and a fabric.mod.json
    // TODO: Parse both files and determine the actual loader
    // TODO: Add support for other loaders
    public void ScanFiles(string packPath)
    {
        GD.Print($"[SCANNER] [INFO] Scanning mods for pack: {Path.GetFileName(packPath)}");

        string manifestPath = $"{packPath}/manifest.toml";
        string modDirectoryPath = $"{packPath}/mods";

        manifest = PackManifest.LoadOrCreate(manifestPath);
        modFilePaths = Directory.EnumerateFiles(modDirectoryPath);

        totalFileCount = modFilePaths.Count();

        // Main loop
        foreach (string filePath in modFilePaths)
        {
            ScanFile(filePath);
        }

        // Summary of the scan
        GD.Print(
            $"[SCANNER] [INFO] {modsScannedCount} mods scanned out of {totalFileCount} files in folder: {failedScanCount} failed scan(s)"
        );

        manifest.SaveToFile(manifestPath);
    }

    private void ScanFile(string filePath)
    {
        string fileHash = Hash(filePath);
        string fileName = Path.GetFileName(filePath);
        ModEntry modEntry;

        try
        {
            using var stream = File.OpenRead(filePath);
            using var jar = new ZipArchive(stream, ZipArchiveMode.Read);

            if (manifest.Mods.ContainsKey(fileHash))
            {
                // Log progress
                modsScannedCount++;
                LogProgress(fileName, $"File already in manifest");
            }
            // Scan forge mod toml file
            else if (jar.GetEntry("META-INF/mods.toml") is ZipArchiveEntry forgeEntry)
            {
                // Parse data and add <fileHash, modEntry> to manifest's mods dictonary
                modEntry = Parser.ParseForge(forgeEntry, filePath);
                manifest.Mods[fileHash] = modEntry;

                // Log progress
                modsScannedCount++;
                LogProgress(fileName, $"Forge mod scanned");
            }
            // Scan fabric mod json file
            else if (jar.GetEntry("fabric.mod.json") is ZipArchiveEntry fabricEntry)
            {
                // Parse data and add <fileHash, modEntry> to manifest's mods dictonary
                modEntry = Parser.ParseFabric(fabricEntry, filePath);
                manifest.Mods[fileHash] = modEntry!;

                // Log progress
                modsScannedCount++;
                LogProgress(fileName, $"Fabric mod scanned");
            }
            // Catch other mod loaders
            else
            {
                // Log progress
                failedScanCount++;
                LogProgress(fileName, $"Failed to scan file: unsupported mod loader");
            }
        }
        catch (Exception ex)
        {
            // Log progress
            failedScanCount++;
            LogProgress(fileName, $"Failed to scan file: {ex.Message}");
        }
    }

    private void LogProgress(string fileName, string message)
    {
        GD.Print(
            $"[SCANNER] [PROGRESS] [{ScannedFileCount()}/{totalFileCount}] {message}: {fileName}"
        );
    }

    public void Dispose() => GD.Print($"[SCANNER] [INFO] Scanner disposed");
}
