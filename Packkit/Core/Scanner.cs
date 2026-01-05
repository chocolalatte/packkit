using System.Dynamic;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Net.Http.Json;
using System.Net.Mime;
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
        Manifest manifest = Manifest.LoadFromFile(@"../manifest.toml");
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
            string fileHash = Hasher.Hash(modFile);
            string fileName = Path.GetFileName(modFile);
            ModEntry? modEntry = null;

            try
            {
                using var stream = File.OpenRead(modFile);
                using var jar = new ZipArchive(stream, ZipArchiveMode.Read);

                if (jar.GetEntry("META-INF/mods.toml") is ZipArchiveEntry forgeEntry)
                {
                    modEntry = Parser.ParseForge(forgeEntry, modFile);
                    manifest.Mods[fileHash] = modEntry;

                    forgeModCount++;
                    Console.WriteLine(
                        $"[{scannedFileCount()}/{totalFileCount}] [SCANNER] [INFO] Forge mod scanned: {fileName}"
                    );
                }
                else if (jar.GetEntry("fabric.mod.json") is ZipArchiveEntry fabricEntry)
                {
                    modEntry = Parser.ParseFabric(fabricEntry, modFile);
                    manifest.Mods[fileHash] = modEntry;

                    fabricModCount++;
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

        manifest.SaveToFile();
    }
}
