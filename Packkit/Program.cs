using Packkit.Core;
using Packkit.Core.Manifest;

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
            TestTags();
            // TestSort();
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

        static void TestTags()
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
