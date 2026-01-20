using System;
using Godot;
using static Packkit.Tags.TagDefinitions;

public partial class EnumTagEntry : HBoxContainer
{
    [Export]
    private Label TagNameLabel;

    [Export]
    private OptionButton optionButton;

    public void Initialize(EnumTagDefinition tag)
    {
        TagNameLabel.Text = tag.Name;
        foreach (string option in tag.Options)
        {
            optionButton.AddItem(option);
        }
    }
}
