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

    private bool isChanged = false;

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
                $"[TAGENTRY:VALUETAGENTRY] [ERROR-001] Unsupported tag type: {Tag.Type}"
            );
        }
    }

    public object GetValue()
    {
        if (Tag.Type == ValueTagType.Integer || Tag.Type == ValueTagType.Float)
        {
            return NumberInput.Value;
        }
        else if (Tag.Type == ValueTagType.String)
        {
            if (TextInput.Text != "")
            {
                return TextInput.Text;
            }
        }
        else
        {
            throw new NotImplementedException(
                $"[TAGENTRY:VALUETAGENTRY] [ERROR-003] Unsupported tag type: {Tag.Type}"
            );
        }

        return null;
    }

    private void _on_number_input_value_changed(int value) => isChanged = true;

    private void _on_hidden()
    {
        isChanged = false;
        TextInput.Text = "";
        NumberInput.Value = 0;
    }
}
