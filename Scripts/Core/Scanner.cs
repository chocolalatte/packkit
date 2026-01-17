using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mime;
using Godot;
using Microsoft.VisualBasic;
using Packkit.Manifest;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core;

public class Scanner
{
    // Some files may have both a mods.toml and a fabric.mod.json
    // TODO: Parse both files and determine the actual loader
    // TODO: Add support for other loaders
    // TODO: Clean up code, extract functions to make more readable
    public static void ScanFiles(string packPath)
    {
        string manifestPath = $"{packPath}/manifest.toml";
        string modsDirectoryPath = $"{packPath}/mods";

        PackManifest manifest = PackManifest.LoadOrCreate(manifestPath);
        var modsDirectory = Directory.EnumerateFiles(modsDirectoryPath);

        // Variables for keeping track of progress
        int totalFileCount = modsDirectory.Count();
        int failedScanCount = 0;
        int modsScannedCount = 0;

        // Function to return the total number of files scanned
        int scannedFileCount()
        {
            return modsScannedCount + failedScanCount;
        }

        // Main loop
        foreach (var file in modsDirectory)
        {
            string fileHash = Hasher.Hash(file);
            string fileName = Path.GetFileName(file);
            ModEntry modEntry = null;

            try
            {
                using var stream = File.OpenRead(file);
                using var jar = new ZipArchive(stream, ZipArchiveMode.Read);

                if (manifest.Mods.ContainsKey(fileHash))
                {
                    modsScannedCount++;
                    // Log progress
                    GD.Print(
                        $"[SCANNER] [PROGRESS] [{scannedFileCount()}/{totalFileCount}] File already in manifest: {fileName}"
                    );
                    continue;
                }

                // Scan forge mod toml file
                if (jar.GetEntry("META-INF/mods.toml") is ZipArchiveEntry forgeEntry)
                {
                    // Parse data and add <fileHash, modEntry> to manifest's mods dictonary
                    modEntry = Parser.ParseForge(forgeEntry, file);
                    manifest.Mods[fileHash] = modEntry!;

                    // Log progress
                    modsScannedCount++;
                    GD.Print(
                        $"[SCANNER] [PROGRESS] [{scannedFileCount()}/{totalFileCount}] Forge mod scanned: {fileName}"
                    );
                }
                // Scan fabric mod json file
                else if (jar.GetEntry("fabric.mod.json") is ZipArchiveEntry fabricEntry)
                {
                    // Parse data and add <fileHash, modEntry> to manifest's mods dictonary
                    modEntry = Parser.ParseFabric(fabricEntry, file);
                    manifest.Mods[fileHash] = modEntry!;

                    // Log progress
                    modsScannedCount++;
                    GD.Print(
                        $"[SCANNER] [PROGRESS] [{scannedFileCount()}/{totalFileCount}] Fabric mod scanned: {fileName}"
                    );
                }
                // Catch other mod loaders
                else
                {
                    // Log progress
                    failedScanCount++;
                    GD.Print(
                        $"[SCANNER] [PROGRESS] [{scannedFileCount()}/{totalFileCount}] failed to scan file: {fileName}, unsupported mod loader"
                    );
                }
            }
            catch
            {
                // Log progress
                failedScanCount++;
                GD.Print(
                    $"[SCANNER] [PROGRESS] [{scannedFileCount()}/{totalFileCount}] failed to scan file: {fileName}"
                );
            }
        }

        // Summary of the scan
        GD.Print(
            $"[SCANNER] [INFO] {modsScannedCount} mods scanned out of {totalFileCount} files in folder: {failedScanCount} failed scan(s)"
        );

        manifest.SaveToFile(manifestPath);
    }
}
