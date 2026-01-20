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

    public ValueTagDefinition Tag;

    public void Initialize(ValueTagDefinition tag)
    {
        Tag = tag;
        TagNameLabel.Text = Tag.Name;
        if (Tag.Type == ValueTagType.Integer || Tag.Type == ValueTagType.Float)
        {
            NumberInput.Visible = true;
        }
        else if (Tag.Type == ValueTagType.String)
        {
            GD.Print("string type");
            TextInput.Visible = true;
        }
        else
        {
            throw new NotImplementedException(
                $"[TAGENTRY:VALUETAGENTRY] Unsupported tag type: {Tag.Type}"
            );
        }
    }

    public object GetValue()
    {
        return Tag.Type switch
        {
            ValueTagType.Integer => NumberInput.Value,
            ValueTagType.Float => NumberInput.Value,
            ValueTagType.String => TextInput.Text,
            _ => throw new NotImplementedException(
                $"[TAGENTRY:VALUETAGENTRY] Unsupported tag type: {Tag.Type}"
            ),
        };
    }
}
