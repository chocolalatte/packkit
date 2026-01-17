using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Godot;
using Packkit.Manifest;
using Tomlyn;

namespace Packkit.PackManagement
{
    public static class PackManager
    {
        public static PackManifest? ActivePack { get; private set; }
        public static Dictionary<Guid, PackManifest> Packs { get; private set; } = [];
        private static readonly string packsFolder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "Pakkit",
            "packs"
        );

        public static void SetActivePack(Guid packId) => ActivePack = Packs[packId];

        public static void Initialize()
        {
            LoadAllPacks();
        }

        private static void LoadAllPacks()
        {
            Console.WriteLine("[PACKMANAGEMENT:PACKMANAGER] [INFO] Loading all packs");

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
                    Console.WriteLine(
                        $"[PACKMANAGEMENT:PACKMANAGER] [PROGRESS] [{scannedPackCount()}/{totalPackCount}] Successfully loaded pack \"{packDirectoryName}\""
                    );
                }
                catch (Exception exception)
                {
                    // Log progress and error
                    failedScanCount++;
                    Console.WriteLine(
                        $"[PACKMANAGEMENT:PACKMANAGER] [PROGRESS] [{scannedPackCount()}/{totalPackCount}] Failed to load pack \"{packDirectoryName}\" reason: \"{exception.Message}\""
                    );
                    continue;
                }
            }

            // Log summary
            Console.WriteLine(
                $"[PACKMANAGEMENT:PACKMANAGER] [INFO] {packsScannedCount} out of {totalPackCount} packs loaded successfully: {failedScanCount} packs failed"
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
                    throw new Exception(
                        $"[PACKMANAGEMENT:PACKMANAGER] [ERROR-003] No active pack selected"
                    );
                }

                guid = Guid.Parse(ActivePack.Header.Id);
            }

            string name = Packs[guid.Value].Header.Name;
            string packPath = Path.Combine(packsFolder, name!);

            // Check if the pack exists
            if (!Directory.Exists(packPath))
            {
                throw new DirectoryNotFoundException(
                    $"[PACKMANAGEMENT:PACKMANAGER] [ERROR-001] PackManifest for pack \"{name}\" with guid \"{guid}\" not found at path {packPath}"
                );
            }

            return packPath;
        }

        public static Guid CreatePack(string packName, string packAuthor)
        {
            Console.WriteLine($"[PACKMANAGEMENT:PACKMANAGER] [INFO] Creating pack \"{packName}\"");
            string packPath = Path.Combine(packsFolder, packName);

            // Check if pack already exists
            if (Directory.Exists(packPath))
            {
                throw new Exception(
                    $"[PACKMANAGEMENT:PACKMANAGER] [ERROR-002] Pack \"{packName}\" already exists"
                );
            }

            Directory.CreateDirectory(packPath);
            Directory.CreateDirectory(packPath + "/mods");

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
                $"[PACKMANAGEMENT:PACKMANAGER] [INFO] Pack \"{packName}\" created successfully with slug {packSlug}"
            );

            return guid;
        }

        public static void DeletePack(Guid packId)
        {
            GD.Print(
                $"[PACKMANAGEMENT:PACKMANAGER] [INFO] Deleting pack \"{Packs[packId].Header.Name}\""
            );

            string packPath = GetPackFolderPath(packId);
            Directory.Delete(packPath, true);
            Packs.Remove(packId);
        }
    }
}
