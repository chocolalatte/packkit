namespace Packkit.Core.Manifest;

public class Customization
{
    public TagRegistry Tags { get; set; } = null!;

    // Simple user defined tag, e.g. "technology", "decoration", etc.
    public class SimpleTagDefinition
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
    }

    // User defined tag with value pairs, e.g. priority = 0
    public class ValueTagDefinition
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public object? DefaultValue { get; set; } = null;
    }

    // User defined tag with enum values, e.g. decoration-type = "building blocks" || "furniture"
    public class EnumTagDefinition
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string[] Options { get; set; } = [];
        public object? Default { get; set; } = null;
    }

    public class TagRegistry
    {
        public List<SimpleTagDefinition> SimpleTags { get; set; } = [];
        public List<ValueTagDefinition> ValueTags { get; set; } = [];
        public List<EnumTagDefinition> EnumTags { get; set; } = [];
    }
}
