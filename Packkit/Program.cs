using Packkit.Core;
using Packkit.Core.Manifest;

namespace Packkit
{
    class Program
    {
        static void Main()
        {
            // Utils.SaveBaseManifest();
            // Scanner.ScanFiles();
            TestTags();
        }

        static void TestTags()
        {
            var manifest = PackManifest.LoadFromFile(@"../manifest.toml");

            var tag = manifest.Customization.Tags.SimpleTags[0];

            Tagger.AddSimpleTag(
                tag.Name,
                manifest.Mods["537f84f1153369d61878965846a8ead7c5eae3a6ad917b04c02521ad76182ad2"],
                manifest
            );
        }
    }
}
