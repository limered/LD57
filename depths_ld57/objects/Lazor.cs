using Godot;
using System;

public partial class Lazor : Node2D
{
	[Export]
	private LazorRaycast lazorCast;

	[Export]
	private Sprite2D lazorLine;

	[Export]
	private CpuParticles2D lazorEmitterParticles;

	[Export]
	private CpuParticles2D lazorBeamParticles;

	[Export]
	private float lazorSpeed = 0.2f;

	[Export]
	public float LazorWidth { get; set; } = 50f;

	[Export]
	public float LazorLength { get; set; } = 500f;

	private Vector2 direction = Vector2.Zero;
	private bool firing = false;

	private const float LazorSpriteWidth = 100f;
	private const float LazorSpriteHeight = 24f;

	public override void _Ready()
	{
		AnimateLazor(false);
	}

	public override void _Process(double delta)
	{
		HandleLazorDirection(delta);

		if (direction == Vector2.Zero)
		{
			firing = false;
			lazorLine.Position = new Vector2(0, 0);
			// lazorLine.Scale = new Vector2(0, 0);
			return;
		}
		else
		{
			firing = true;
			lazorCast.TargetPosition = direction * LazorLength;
			lazorCast.ForceRaycastUpdate();

			lazorEmitterParticles.Rotation = direction.Angle();

			lazorLine.Position = direction * LazorLength / 2;
			lazorLine.Rotation = direction.Angle();

			lazorBeamParticles.Rotation = direction.Angle();
			lazorBeamParticles.Position = lazorLine.Position;
			lazorBeamParticles.EmissionRectExtents = new Vector2(lazorLine.Position.X, LazorWidth);
		}
	}

	private void AnimateLazor(bool enabled)
	{
		var tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.In);
		tween.TweenProperty(lazorLine, "scale",
			new Vector2(
				LazorLength / LazorSpriteWidth , 
				enabled
					? LazorWidth / LazorSpriteHeight
					: 0f
			),
			enabled ? lazorSpeed : lazorSpeed / 2
		);

		lazorEmitterParticles.Emitting = enabled;
		lazorBeamParticles.Emitting = enabled;
		lazorLine.Visible = enabled;
	}

	private void HandleLazorDirection(double delta)
	{
		var newDirection = Vector2.Zero;
		if (Input.IsActionPressed("lazor_up"))
			newDirection += Vector2.Up;
		if (Input.IsActionPressed("lazor_down"))
			newDirection += Vector2.Down;
		if (Input.IsActionPressed("lazor_left"))
			newDirection += Vector2.Left;
		if (Input.IsActionPressed("lazor_right"))
			newDirection += Vector2.Right;

		if (newDirection != Vector2.Zero)
		{
			newDirection = newDirection.Normalized();
			if (newDirection != direction)
			{
				AnimateLazor(true);
				direction = newDirection;
			}
		}
		else if (direction != Vector2.Zero)
		{
			AnimateLazor(false);
			direction = Vector2.Zero;
		}
	}
}
