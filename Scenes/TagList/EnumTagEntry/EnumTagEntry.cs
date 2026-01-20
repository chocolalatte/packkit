using System;
using System.Collections.Generic;
using Godot;
using static Packkit.Tags.TagDefinitions;

public partial class EnumTagEntry : HBoxContainer
{
    [Export]
    private Label TagNameLabel;

    [Export]
    private OptionButton optionButton;

    public Dictionary<int, string> TagOptions = [];

    public EnumTagDefinition Tag;

    public void Initialize(EnumTagDefinition tag)
    {
        Tag = tag;
        TagNameLabel.Text = Tag.Name;

        TagOptions.Add(0, "none");

        foreach (string option in Tag.Options)
        {
            // Add option to dictionary before adding to option button since index starts at 0 and item count starts at 1
            TagOptions.Add(optionButton.GetItemCount(), option);
            optionButton.AddItem(option);
        }
    }

    public string GetValue()
    {
        if (optionButton.Selected == 0)
        {
            return null;
        }

        return TagOptions[optionButton.Selected];
    }

    private void _on_hidden()
    {
        optionButton.Selected = 0;
    }
}
