using Packkit.Manifest;

namespace Packkit.Core;

// TODO: Add methods for value and enum tags
public static class Sorter
{
    public static IEnumerable<ModEntry> WithSimpleTags(
        IEnumerable<ModEntry> mods,
        IEnumerable<string> requiredTags,
        bool requireAll = true
    )
    {
        var tagSet = requiredTags.ToHashSet();

        return mods.Where(mod =>
            requireAll
                ? tagSet.All(t => mod.Tags.Simple.Contains(t))
                : tagSet.Any(t => mod.Tags.Simple.Contains(t))
        );
    }
}
