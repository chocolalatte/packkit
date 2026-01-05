using System.Dynamic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Net.Http.Json;
using Microsoft.VisualBasic;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core;

public class Scanner
{
    // Some files may have both a mods.toml and a fabric.mod.json
    // TODO: Parse both files and determine the actual loader
    public static void ScanFiles(string modsDirectoryPath = @"../mods")
    {
        int totalFileCount;

        int failedScanCount = 0;
        int forgeModCount = 0;
        int fabricModCount = 0;

        int scannedFileCount()
        {
            return forgeModCount + fabricModCount + failedScanCount;
        }

        var modsDirectory = Directory.EnumerateFiles(modsDirectoryPath);

        totalFileCount = modsDirectory.Count();

        foreach (var modFile in modsDirectory)
        {
            string fileName = Path.GetFileName(modFile);

            try
            {
                using var stream = File.OpenRead(modFile);
                using var jar = new ZipArchive(stream, ZipArchiveMode.Read);

                if (jar.GetEntry("META-INF/mods.toml") is ZipArchiveEntry forgeEntry)
                {
                    forgeModCount++;
                    ScanForgeEntry(forgeEntry);
                    Console.WriteLine(
                        $"[{scannedFileCount()}/{totalFileCount}] [SCANNER] [INFO] Forge mod scanned: {fileName}"
                    );
                }
                else if (jar.GetEntry("fabric.mod.json") is ZipArchiveEntry fabricEntry)
                {
                    fabricModCount++;
                    ScanFabricEntry(fabricEntry);
                    Console.WriteLine(
                        $"[{scannedFileCount()}/{totalFileCount}] [SCANNER] [INFO] Fabric mod scanned: {fileName}"
                    );
                }
            }
            catch
            {
                failedScanCount++;
                Console.WriteLine(
                    $"[{scannedFileCount()}/{totalFileCount}] [SCANNER] [ERROR-001] failed to scan file: {fileName}"
                );
            }
        }

        Console.WriteLine(
            $"[SCANNER] [INFO] {fabricModCount + forgeModCount} mods scanned out of {totalFileCount} files in folder: {failedScanCount} failed scan(s)"
        );
    }

    private static void ScanForgeEntry(ZipArchiveEntry forgeEntry)
    {
        using var reader = new StreamReader(forgeEntry.Open());
        string modsToml = reader.ReadToEnd();

        TomlTable model = Toml.ToModel(modsToml);

        if (!model.TryGetValue("mods", out var modsObj))
            return;

        if (modsObj is not TomlTableArray modsArray)
            return;

        foreach (TomlTable mod in modsArray)
        {
            if (mod.TryGetValue("modId", out var idObj))
            {
                string? modId = idObj.ToString();
            }
        }
    }

    private static void ScanFabricEntry(ZipArchiveEntry fabricEntry)
    {
        using var reader = new StreamReader(fabricEntry.Open());
        string modsJson = reader.ReadToEnd();
    }
}
