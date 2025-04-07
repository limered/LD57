using Godot;
using System;

public partial class NoseSubmarine : TextureRect
{
	public override void _Ready()
	{
		var player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.AnimationFinished += (StringName animName) => {
			GD.Print("Animation finished");
			player.SpeedScale = -1;
			player.Play("brainwash_animations/dive_into_nose", fromEnd: true);	
		};
		player.SpeedScale = 1;
		player.Play("brainwash_animations/dive_into_nose");
	}

	public override void _Process(double delta)
	{

	}
}
