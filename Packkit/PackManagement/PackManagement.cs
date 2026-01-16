using System.Net;
using Packkit.Manifest;
using Tomlyn;

namespace Packkit.PackManagement
{
    public static class PackManager
    {
        public static PackManifest? ActivePack { get; private set; }
        public static Dictionary<Guid, PackManifest> Packs { get; private set; } = [];
        private static readonly string modpacksFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Pakkit",
            "Modpacks"
        );

        public static void Initialize()
        {
            Packs.Clear();

            foreach (var packDirectory in Directory.EnumerateDirectories(modpacksFolder))
            {
                var manifestPath = Path.Combine(packDirectory, "manifest.toml");

                try
                {
                    PackManifest manifest = PackManifest.LoadExisting(manifestPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    continue;
                }
            }
        }

        public static string? GetPackManifestPath(string name)
        {
            string manifestPath = Path.Combine(modpacksFolder, name, "manifest.toml");

            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException(
                    $"[PACKMANAGEMENT:PACKMANAGER] [ERROR-001] PackManifest for pack \"{name}\" not found",
                    manifestPath
                );
            }

            return manifestPath;
        }

        // TODO: Check if it's overwritting an existing pack
        public static void CreatePack(string packName, string packAuthor)
        {
            Console.WriteLine($"[PACKMANAGEMENT:PACKMANAGER] [INFO] Creating pack \"{packName}\"");
            string packPath = Path.Combine(modpacksFolder, packName);

            if (Directory.Exists(packPath))
            {
                Console.WriteLine(
                    $"[PACKMANAGEMENT:PACKMANAGER] [ERROR-002] Pack \"{packName}\" already exists"
                );
                return;
            }

            Directory.CreateDirectory(packPath);

            PackManifest manifest = Toml.ToModel<PackManifest>(Defaults.BaseManifest);

            manifest.Header.Name = packName;
            manifest.Header.Author = packAuthor;
            manifest.Header.Slug = $"{packAuthor}.{packName}".ToLower().Replace(" ", "-");
            if (manifest.Header.Id == Guid.Empty.ToString())
            {
                manifest.Header.Id = Guid.NewGuid().ToString();
            }

            manifest.SaveToFile(packPath + "/manifest.toml");
        }

        // private static void LoadPackManifest(string path)
        // {
        //     PackManifest manifest = Toml.ToModel<PackManifest>(File.ReadAllText(path));
        //     PackManifests.Add(manifest.Header.Name, manifest);
        // }
    }
}
