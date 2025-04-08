using Godot;
using System;

public partial class Follow : Camera2D
{
	[Export]
	public Node2D Target;

	[Export]
	public float FollowDelay = 1f;
	[Export]
	public float FollowSpeed = 2f;

	private Tween tween = null;

	public override void _Ready()
	{
		tween = GetTree().CreateTween();
		tween.TweenInterval(FollowDelay);
		tween.SetTrans(Tween.TransitionType.Elastic);
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(this, "global_position", Target.GlobalPosition, 1 / FollowSpeed);
	}


	public override void _Process(double delta)
	{
		if (Target.GlobalPosition != GlobalPosition)
		{
			tween = GetTree().CreateTween();
			tween.TweenInterval(FollowDelay);
			// tween.SetTrans(Tween.TransitionType.Elastic);
			// tween.SetEase(Tween.EaseType.InOut);
			tween.TweenProperty(this, "global_position", Target.GlobalPosition, 1 / FollowSpeed);
		}
	}
}
