using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Packkit.Core;

namespace Packkit
{
    class Program
    {
        static void Main()
        {
            var manifest = new Manifest();
            manifest.Mods["example_mod"] = new ModEntry {
                Name = "Example Mod",
                File = "example.jar",
                Loader = ModLoader.neoforge,
                Side = ModSide.client,
                Version = "1.0.5",
                Importance = ModImportance.optional
            };
            manifest.SaveToFile("test-manifest.toml");

            var loaded = Manifest.LoadFromFile("test-manifest.toml");
            Console.WriteLine($"Loaded mod: {loaded.Mods["example_mod"].Name}");
            Console.WriteLine($"Schema version {loaded.Header.SchemaVersion}");
            Console.WriteLine($"base-schema version {ManifestUtils.GetSchemaVersionFromEmbedded()}");
        }
    }
}
