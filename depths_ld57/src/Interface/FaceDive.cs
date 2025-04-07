using Godot;
using System;

public partial class FaceDive : Control
{
	private NoseSubmarine submarine;
	private AnimationPlayer player;

	private bool DiveIntoVisibility
	{
		get => GetNode<TextureRect>("FaceToDiveInto").Visible;
		set => GetNode<TextureRect>("FaceToDiveInto").Visible = value;
	}
	
	private bool DiveOutOfVisibility
	{
		get => GetNode<TextureRect>("FaceToDiveOutOf").Visible;
		set => GetNode<TextureRect>("FaceToDiveOutOf").Visible = value;
	}

	public override void _Ready()
	{
		submarine = GetNode<NoseSubmarine>("Submarine");
		player = submarine.GetNode<AnimationPlayer>("AnimationPlayer");
		player.Stop();

		submarine.Visible = false;
		DiveIntoVisibility = false;
		DiveOutOfVisibility = false;
	}

	public void PlayIntro(Action andThen)
	{
		submarine.Visible = true;
		DiveIntoVisibility = true;
		DiveOutOfVisibility = false;

		player.AnimationFinished += (StringName animName) =>
		{
			DiveIntoVisibility = false;
			submarine.Visible = false;
			player.Stop();
			andThen();
		};
		player.SpeedScale = 1;
		player.Play("brainwash_animations/dive_into_nose");
	}

	public void PlayOutro()
	{
		submarine.Visible = true;
		DiveIntoVisibility = false;
		DiveOutOfVisibility = true;

		player.SpeedScale = -1;
		player.Play("brainwash_animations/dive_into_nose", fromEnd: true);
	}
}
