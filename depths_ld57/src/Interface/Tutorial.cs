using Godot;

namespace depths_ld57.Interface;

public partial class Tutorial : Control
{
	[Export]
	public float ShowTime = 5;

	public override void _Ready()
	{
		Visible = false;
	}

	public void Start()
	{
		Visible = true;
		GetTree().CreateTimer(ShowTime).Timeout += () => {
			GetNode<Game>("/root/Game").GoToState(GameState.MapGeneration);
		};
	}
}