using Godot;

namespace depths_ld57.Interface;

public partial class InterfaceNode : Control
{
    [Export]
    public FaceDive faceDive;

    public override void _Ready()
    {
        var button = GetNode<TextureButton>("TextureButton");
        button.Pressed += OnStartPressed;
    }

    private void OnStartPressed()
    {        
        faceDive.PlayIntro(() => {
            GetNode<Game>("/root/Game").GoToState(GameState.MapGeneration);
        });
    }
}