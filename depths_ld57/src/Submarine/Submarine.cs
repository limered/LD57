using depths_ld57;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;
using System;

public partial class Submarine : RigidBody2D
{
	[Export]
	public float Acceleration = 100f;

	[Export]
	public float WaterDrag { get => LinearDamp; set => LinearDamp = value; }

	[Export]
	public float SubmarineRadius { get; set; } = 10;

	[Export]
	public float SubmarineLookahead { get; set; } = 10f;

	[Export] public Lazor _lazor;

	private AudioStreamPlayer _audio;
	[Export] private AudioStream _movementAudio;

	public Vector2I GridPosition
	{
		get => new Vector2I((int)GlobalPosition.X + collisionChecker.width / 2, (int)GlobalPosition.Y + collisionChecker.height / 2);
		set => GlobalPosition = new Vector2(value.X - collisionChecker.width / 2, value.Y - collisionChecker.height / 2);
	}

	private AnimatedSprite2D _sprite;

	private CollisionChecker collisionChecker;


	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_audio = GetNode<AudioStreamPlayer>("../Audio");
		EventBus.Register<MapGeneratedEvent>((evt) =>
		{
			var mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
			collisionChecker = new(mapGenerator.CollisionMap);
			while (IsStuck())
			{
				GridPosition = new Vector2I(
					GD.RandRange(0, 2048),
					GD.RandRange(0, 2048)
				);
			}
		});
	}

	public override void _Process(double delta)
	{
		if (_lazor.IsShooting)
		{
			Shake();
			_sprite.Position = Vector2.Zero;
		}
	}

	private float _shakeAmount = 3.0f;
	private float _shakeDuration = 0.042f;

	private void Shake()
	{
		var tween = GetTree().CreateTween();
		var randomOffset = new Vector2(
			GD.Randf() * _shakeAmount * 2 - _shakeAmount,
			GD.Randf() * _shakeAmount * 2 - _shakeAmount
		);

		tween.TweenProperty(_sprite, "position", randomOffset, _shakeDuration);
	}

	public override void _PhysicsProcess(double delta)
	{
		var direction = HandleMovement(delta);

		_sprite.FlipH = LinearVelocity.X > 0;

		if (WouldCollide(LinearVelocity.Normalized()))
		{
			LinearVelocity = -LinearVelocity * 0.5f;
			ApplyCentralForce(-direction * Acceleration * (float)delta);
		}
		else
		{
			ApplyCentralForce(direction * Acceleration * (float)delta);
		}
	}

	private bool WouldCollide(Vector2 moveDirection)
	{
		if (collisionChecker == null)
			return false;
		if (moveDirection == Vector2.Zero) return false;

		if (collisionChecker.IsCollision(GlobalPosition + moveDirection * (SubmarineRadius + SubmarineLookahead)))
		{
			return true;
		}
		return false;
	}

	public bool IsStuck()
	{
		return collisionChecker.IsCollision(GlobalPosition)
			&& collisionChecker.IsCollision(GlobalPosition + Vector2.Right * SubmarineRadius / 2)
			&& collisionChecker.IsCollision(GlobalPosition + Vector2.Down * SubmarineRadius / 2)
			&& collisionChecker.IsCollision(GlobalPosition + Vector2.Left * SubmarineRadius / 2)
			&& collisionChecker.IsCollision(GlobalPosition + Vector2.Up * SubmarineRadius / 2);
	}

	private Vector2 HandleMovement(double delta)
	{
		var direction = Vector2.Zero;
		if (Input.IsActionPressed("move_up"))
			direction += Vector2.Up;
		if (Input.IsActionPressed("move_down"))
			direction += Vector2.Down;
		if (Input.IsActionPressed("move_left"))
			direction += Vector2.Left;
		if (Input.IsActionPressed("move_right"))
			direction += Vector2.Right;

		if (direction != Vector2.Zero)
		{
			direction = direction.Normalized();
		}

		return direction;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key)
		{
			if (key.IsReleased() && key.Keycode == Key.R)
			{
				GridPosition = new Vector2I(
					GD.RandRange(1, 2047),
					GD.RandRange(1, 2047)
				);
			}
			if (key.IsActionPressed("move_up") ||
				key.IsActionPressed("move_down") ||
				key.IsActionPressed("move_left") ||
				key.IsActionPressed("move_right")
			)
			{
				_audio.Stream = _movementAudio;
				_audio.Play();
			}
		}
	}

}


public class CollisionChecker
{
	public readonly Image collisionMap;
	public readonly int width, height;

	public CollisionChecker(Image collisionMap)
	{
		this.collisionMap = collisionMap;
		width = collisionMap.GetWidth();
		height = collisionMap.GetHeight();
	}

	public bool IsCollision(Vector2 position)
	{
		if (position.X <= -width / 2 || position.X >= width / 2 || position.Y <= -height / 2 || position.Y >= height / 2)
			return true;

		var pixel = collisionMap.GetPixel((int)position.X + width / 2, (int)position.Y + height / 2);
		return pixel.R > 0.5f;
	}

}
