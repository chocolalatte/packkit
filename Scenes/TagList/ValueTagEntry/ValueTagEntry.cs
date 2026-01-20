using System;
using Godot;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Godot.TagEntry;

public partial class ValueTagEntry : HBoxContainer
{
    [Export]
    public SpinBox NumberInput;

    [Export]
    public LineEdit TextInput;

    [Export]
    private Label TagNameLabel;

    public ValueTagType TagType;

    public void Initialize(ValueTagDefinition tag)
    {
        TagType = tag.Type;
        TagNameLabel.Text = tag.Name;
        if (tag.Type == ValueTagType.Integer || tag.Type == ValueTagType.Float)
        {
            NumberInput.Visible = true;
        }
        else if (tag.Type == ValueTagType.String)
        {
            GD.Print("string type");
            TextInput.Visible = true;
        }
        else
        {
            throw new NotImplementedException(
                $"[TAGENTRY:VALUETAGENTRY] Unsupported tag type: {tag.Type}"
            );
        }
    }
}
