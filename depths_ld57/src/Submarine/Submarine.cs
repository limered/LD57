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

	public float SubmarineRadius { get; private set; }

	[Export]
	public float SubmarineLookahead { get; set; } = 10f;


	private AnimatedSprite2D _sprite;

	private CollisionChecker collisionChecker;


	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		EventBus.Register<MapGeneratedEvent>((evt) =>
		{
			var mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
			collisionChecker = new(mapGenerator.CollisionMap);
		});
		var capsule = (CapsuleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		SubmarineRadius = (capsule.Radius + capsule.Height) / 2f;
	}

	public override void _PhysicsProcess(double delta)
	{
		var direction = HandleMovement(delta);
		Mathf.Sign(LinearVelocity.X);
		_sprite.FlipH = LinearVelocity.X > 0;
		HandleCollision(direction);
	}

	private void HandleCollision(Vector2 moveDirection)
	{
		if (collisionChecker == null)
			return;
		if (moveDirection == Vector2.Zero) return;

		if (collisionChecker.IsCollision(GlobalPosition + moveDirection * (SubmarineRadius + SubmarineLookahead)))
		{
			LinearVelocity = Vector2.Zero;
			GD.Print("Collision detected with wall");
		}
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
			ApplyCentralForce(direction * Acceleration * (float)delta);
		}
		return direction;
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
		return pixel == new Color(0, 0, 0, 0);
	}

}