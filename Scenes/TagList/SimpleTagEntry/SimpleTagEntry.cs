using System;
using Godot;
using static Packkit.Tags.TagDefinitions;

namespace Packkit.Godot;

public partial class SimpleTagEntry : HBoxContainer
{
    [Export]
    private Label TagNameLabel;

    [Export]
    private CheckBox IsSelectedCheckBox;

    public SimpleTagDefinition Tag;

    public void Initialize(SimpleTagDefinition tag)
    {
        Tag = tag;
        TagNameLabel.Text = Tag.Name;
    }
}
