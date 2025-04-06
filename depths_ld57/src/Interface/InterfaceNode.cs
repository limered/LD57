using Godot;

namespace depths_ld57.Interface;

public partial class InterfaceNode : Control
{
    public override void _Ready()
    {
        var button = GetNode<TextureButton>("TextureButton");
        button.Pressed += OnStartPressed;
    }

    private void OnStartPressed()
    {
        GetNode<RichTextLabel>("RichTextLabel").SetVisible(true);
        GetNode<Game>("/root/Game").GoToState(GameState.MapGeneration);
    }
}