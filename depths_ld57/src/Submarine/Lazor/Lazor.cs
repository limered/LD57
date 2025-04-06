using depths_ld57;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

public partial class Lazor : Node2D
{
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

	[Export]
	public float MaxLazorLength { get; set; } = 1000f;

	private Vector2 direction = Vector2.Zero;
	private bool firing = false;

	private const float LazorSpriteWidth = 100f;
	private const float LazorSpriteHeight = 24f;

	private CollisionChecker collisionChecker;

	public override void _Ready()
	{
		EventBus.Register<MapGeneratedEvent>((evt) =>
		{
			var mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
			collisionChecker = new(mapGenerator.CollisionMap);
		});
	}

	public override void _Process(double delta)
	{
		HandleLazorDirection(delta);

		if (direction == Vector2.Zero)
		{
			firing = false;
			lazorLine.Position = new Vector2(0, 0);
		}
		else
		{
			firing = true;

			UpdateLazorLength();

			lazorLine.Position = direction * LazorLength / 2;
			lazorLine.Rotation = direction.Angle();
			lazorLine.Scale = new Vector2(LazorLength / LazorSpriteWidth, LazorWidth / LazorSpriteHeight);

			lazorBeamParticles.Rotation = direction.Angle();
			lazorBeamParticles.Position = lazorLine.Position;
			lazorBeamParticles.EmissionRectExtents = new Vector2(LazorLength / 2, LazorWidth / 10);

			lazorEmitterParticles.Rotation = direction.Angle();
		}

		lazorLine.Visible = firing;
		lazorBeamParticles.Emitting = firing;
		lazorEmitterParticles.Emitting = firing;
	}

	private void UpdateLazorLength()
	{
		if (collisionChecker == null)
			return;
		if (direction == Vector2.Zero)
			return;

		var step = direction * LazorLength / 100;
		for (
			var currentPos = GlobalPosition;
			(currentPos - GlobalPosition).LengthSquared() < MaxLazorLength * MaxLazorLength;
			currentPos += step
		)
		{
			if (collisionChecker.IsCollision(currentPos))
			{
				LazorLength = (currentPos - GlobalPosition).Length();
				return;
			}
		}
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
			direction = newDirection;
		}
		else if (direction != Vector2.Zero)
		{
			direction = Vector2.Zero;
			LazorLength = 10;
		}
	}
}
