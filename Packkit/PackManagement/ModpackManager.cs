using Packkit.Manifest;
using Tomlyn;

namespace Packkit.PackManagement
{
    public class PackManager
    {
        public static void CreatePack(string name)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string modpacksFolder = Path.Combine(appData, "Pakkit", "Modpacks", name);

            Directory.CreateDirectory(modpacksFolder);

            PackManifest manifest = Toml.ToModel<PackManifest>(Defaults.BaseManifest);

            manifest.Header.Name = name;

            manifest.SaveToFile(modpacksFolder + "/manifest.toml");
        }
    }
}
