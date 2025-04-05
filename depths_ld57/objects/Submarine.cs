using Godot;
using System;

public partial class Submarine : RigidBody2D
{
	[Export]
	public float Acceleration = 100f;

	[Export]
	public float WaterDrag { get => LinearDamp; set => LinearDamp = value; }

	private AnimatedSprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
		HandleMovement();
		Mathf.Sign(LinearVelocity.X);
		_sprite.FlipH = LinearVelocity.X > 0;
	}

	private void HandleMovement()
	{
		var direction = Vector2.Zero;
		if (Input.IsActionPressed("up"))
			direction += Vector2.Up;
		if (Input.IsActionPressed("down"))
			direction += Vector2.Down;
		if (Input.IsActionPressed("left"))
			direction += Vector2.Left;
		if (Input.IsActionPressed("right"))
			direction += Vector2.Right;

		if (direction != Vector2.Zero)
		{
			direction = direction.Normalized();
			ApplyForce(direction * Acceleration);
		}
	}
}
