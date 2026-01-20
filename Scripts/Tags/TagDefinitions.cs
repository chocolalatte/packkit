namespace Packkit.Tags;

public static class TagDefinitions
{
    // Helper classes
    public sealed class ValueTagInstance(ValueTagDefinition definition, object value)
    {
        public ValueTagDefinition Definition = definition;
        public object Value = value;
    }

    public sealed class EnumTagInstance(EnumTagDefinition definition, string value)
    {
        public EnumTagDefinition Definition = definition;
        public string Value = value;
    }

    // Simple user defined tag, e.g. "technology", "decoration", etc.
    public class SimpleTagDefinition
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null;
    }

    // User defined tag with value pairs, e.g. priority = 0
    public class ValueTagDefinition
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null;
        public ValueTagType Type { get; set; }
        public object DefaultValue { get; set; } = null;
    }

    // User defined tag with enum values, e.g. decoration-type = "building blocks" || "furniture"
    public class EnumTagDefinition
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null;
        public string[] Options { get; set; } = [];
        public object Default { get; set; } = null;
    }

    public enum ValueTagType
    {
        Integer,
        Float,
        String,
        Boolean,
    }
}
