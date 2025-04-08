using Godot;
using System;

public partial class FaceDive : Control
{
	private NoseSubmarine submarine;
	private AnimationPlayer player;

	public Action OnIntroFinished { get; set; }
	public Action OnOutroFinished { get; set; }

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

	public void PlayIntro()
	{
		submarine.Visible = true;
		DiveIntoVisibility = true;
		DiveOutOfVisibility = false;

		player.AnimationFinished += FinishedIntro;
		player.SpeedScale = 1;
		player.Play("brainwash_animations/dive_into_nose");
	}

	private void FinishedIntro(StringName animName)
	{
		DiveIntoVisibility = false;
		submarine.Visible = false;
		player.Stop();
		player.AnimationFinished -= FinishedIntro;
		OnIntroFinished?.Invoke();
	}

	public void PlayOutro()
	{
		submarine.Visible = true;
		DiveIntoVisibility = false;
		DiveOutOfVisibility = true;

		player.AnimationFinished += FinishedOutro;

		player.SpeedScale = -1;
		player.Play("brainwash_animations/dive_into_nose", fromEnd: true);
	}

	private void FinishedOutro(StringName animName)
	{
		DiveIntoVisibility = false;
		DiveOutOfVisibility = true;
		submarine.Visible = false;

		player.Stop();
		player.AnimationFinished -= FinishedOutro;
		OnOutroFinished?.Invoke();
	}
}
