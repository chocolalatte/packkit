using System.Collections.Generic;
using Packkit.Tags;

namespace Packkit.Manifest;

public class Customization
{
    public TagRegistry Tags { get; set; } = null!;

    public class TagRegistry
    {
        public List<TagDefinitions.SimpleTagDefinition> SimpleTags { get; set; } = [];
        public List<TagDefinitions.ValueTagDefinition> ValueTags { get; set; } = [];
        public List<TagDefinitions.EnumTagDefinition> EnumTags { get; set; } = [];
    }
}
