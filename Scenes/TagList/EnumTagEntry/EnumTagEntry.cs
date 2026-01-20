using System;
using Godot;
using static Packkit.Tags.TagDefinitions;

public partial class EnumTagEntry : HBoxContainer
{
    [Export]
    private Label TagNameLabel;

    [Export]
    private OptionButton optionButton;

    public EnumTagDefinition Tag;

    public void Initialize(EnumTagDefinition tag)
    {
        Tag = tag;
        TagNameLabel.Text = Tag.Name;
        foreach (string option in Tag.Options)
        {
            optionButton.AddItem(option);
        }
    }

    public string GetValue()
    {
        throw new NotImplementedException();
    }
}
