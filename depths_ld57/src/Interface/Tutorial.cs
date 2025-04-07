using depths_ld57;
using Godot;
using System;

public partial class Tutorial : Control
{
	[Export]
	public float showTime = 5;

    public override void _Ready()
    {
        Visible = false;
    }

	public void Start()
	{
		Visible = true;
		GetTree().CreateTimer(showTime).Timeout += () => {
			Visible = false;
			GetNode<Game>("/root/Game").GoToState(GameState.MapGeneration);
		};
	}
}
