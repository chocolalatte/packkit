using Packkit.Core;
using Packkit.Manifest;
using Packkit.PackManagement;
using Packkit.Tags;

namespace Packkit
{
    class Program
    {
        // Add basic IO
        // Connect everything together in a way that's user friendly
        static void Main()
        {
            // Utils.SaveBaseManifest();
            // Scanner.ScanFiles();
            // TestSimpleTags();
            // TestSort();
            // TestValueTags();
            // TestEnumTags();

            PackManager.CreatePack("TestPack");
        }

        static void TestEnumTags()
        {
            var manifest = PackManifest.LoadFromFile(@"../manifest.toml");
            var mod = manifest.Mods[
                "b6b586eb6fdc9ce4b3645cab43642ba87817c5deadef4d87ab884e4fe20ef282"
            ];
            Tagger.AddEnumTag("importance", "required", mod, manifest);
        }

        static void TestValueTags()
        {
            var manifest = PackManifest.LoadFromFile(@"../manifest.toml");
            var mod = manifest.Mods[
                "b6b586eb6fdc9ce4b3645cab43642ba87817c5deadef4d87ab884e4fe20ef282"
            ];
            Tagger.AddValueTag("priority", 1, mod, manifest);
        }

        static void TestSort()
        {
            PackManifest manifest = PackManifest.LoadFromFile(@"../manifest.toml");
            var mods = manifest.Mods.Values.ToList();

            var SortedMods = Sorter
                .WithSimpleTags(mods, manifest.Customization.Tags.SimpleTags.Select(t => t.Name))
                .ToList();

            SortedMods.ForEach(m => Console.WriteLine(m.Name));
        }

        static void TestSimpleTags()
        {
            var manifest = PackManifest.LoadFromFile(@"../manifest.toml");

            var tag = manifest.Customization.Tags.SimpleTags[0];

            Tagger.AddSimpleTag(
                tag.Name,
                manifest.Mods["b6b586eb6fdc9ce4b3645cab43642ba87817c5deadef4d87ab884e4fe20ef282"],
                manifest
            );
        }
    }
}
