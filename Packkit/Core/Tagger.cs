using Packkit.Core.Manifest;

namespace Packkit.Core;

public static class Tagger
{
    public static void AddSimpleTag(
        string tag,
        ModEntry modEntry,
        PackManifest manifest,
        bool save = true
    )
    {
        var simpleTagDefinitions = manifest.Customization.Tags.SimpleTags;

        // Check if tag exists in manifest's tag definitions
        if (simpleTagDefinitions.Any(t => t.Name == tag))
        {
            if (modEntry.Tags.Simple.Contains(tag))
            {
                Console.WriteLine(
                    $"[MANIFEST:TAGGER] [WARN] Tag {tag} already exists in {modEntry.Name}"
                );
            }
            else
            {
                modEntry.Tags.Simple.Add(tag);

                Console.WriteLine($"[MANIFEST:TAGGER] [INFO] Added tag {tag} to {modEntry.Name}");
                if (save)
                    manifest.SaveToFile();
            }
        }
        else
        {
            Console.WriteLine($"[MANIFEST:TAGGER] [ERROR-001] Tag {tag} not found in manifest");
        }
    }
}
