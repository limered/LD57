using System.Collections.Generic;
using depths_ld57;
using depths_ld57.Dirt;
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
	private CpuParticles2D lazorHitParticles;

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

	private HashSet<DirtParticle> doingDamage = new();

	private Vector2 direction = Vector2.Zero;
	private bool firing = false;
	private LazorTip lazorTip;

	private const float LazorSpriteWidth = 100f;
	private const float LazorSpriteHeight = 24f;

	private CollisionChecker collisionChecker;
	
	private AudioStreamPlayer _audio;

	public override void _Ready()
	{
		lazorTip = (LazorTip)GetNode<Area2D>("LazorTip");
		_audio = GetNode<AudioStreamPlayer>("../../Audio/LazorAudio");
		EventBus.Register<MapGeneratedEvent>((evt) =>
		{
			var mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
			collisionChecker = new(mapGenerator.CollisionMap);
		});
	}

	public override void _Process(double delta)
	{
		HandleLazorDirection();

		UpdateLazorLength();

		if (direction == Vector2.Zero)
		{
			firing = false;
			lazorLine.Position = new Vector2(0, 0);
		}
		else
		{
			firing = true;
			_audio.Play();

			lazorLine.Position = direction * LazorLength / 2;
			lazorLine.Rotation = direction.Angle();
			lazorLine.Scale = new Vector2(LazorLength / LazorSpriteWidth, LazorWidth / LazorSpriteHeight);

			lazorBeamParticles.Rotation = direction.Angle();
			lazorBeamParticles.Position = lazorLine.Position;
			lazorBeamParticles.EmissionRectExtents = new Vector2(LazorLength / 2, LazorWidth / 10);

			lazorEmitterParticles.Rotation = direction.Angle();
			lazorHitParticles.Position = direction * LazorLength;
			lazorHitParticles.Rotation = direction.Angle() + Mathf.Pi;
		}

		lazorLine.Visible = firing;
		lazorBeamParticles.Emitting = firing;
		lazorEmitterParticles.Emitting = firing;
		lazorHitParticles.Emitting = firing && LazorLength < MaxLazorLength;
		lazorTip.Collider.Disabled = !firing;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		foreach (var dirt in doingDamage)
		{
			dirt.Damage();
		}
	}


	private void UpdateLazorLength()
	{
		if (collisionChecker == null)
			return;
		if (direction == Vector2.Zero)
			return;

		//extend lazor length until it hits a wall
		var step = direction * 10; 
		var globalOffset = GlobalPosition - Position;
		for (
			var currentPos = GlobalPosition;
			(currentPos - GlobalPosition).LengthSquared() < MaxLazorLength * MaxLazorLength;
			currentPos += step
		)
		{
			lazorTip.Position = currentPos - globalOffset;
			if (collisionChecker.IsCollision(currentPos))
			{
				LazorLength = (currentPos - GlobalPosition).Length();
				return;
			}
		}
	}

	private void HandleLazorDirection()
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

	public void OnTipCollided(int areaId, Area2D other, int areaShapeIndex, int localShapeIndex)
	{
		if (other is DirtParticle dirt)
		{
			doingDamage.Add(dirt);
		}
	}

	public void OnTipLeave(int areaId, Area2D other, int areaShapeIndex, int localShapeIndex)
	{
		if (other is DirtParticle dirt)
		{
			doingDamage.Remove(dirt);	
		}
	}
}
