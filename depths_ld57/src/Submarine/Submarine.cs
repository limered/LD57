using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Submarine;

public partial class Submarine : RigidBody2D
{
    private AudioStreamPlayer _audio;

    private Game _game;

    [Export] public Lazor Lazor;
    [Export] private AudioStream _movementAudio;

    private float _shakeAmount = 3.0f;
    private float _shakeDuration = 0.042f;

    private AnimatedSprite2D _sprite;

    [Export] public float Acceleration = 100f;

    private CollisionChecker _collisionChecker;
    private MapGenerator _mapGenerator;

    [Export]
    public float WaterDrag
    {
        get => LinearDamp;
        set => LinearDamp = value;
    }

    [Export] public float SubmarineRadius { get; set; } = 10;

    [Export] public float SubmarineLookahead { get; set; } = 10f;

    private Vector2I GridPosition
    {
        get => new((int)GlobalPosition.X + _collisionChecker.Width / 2,
            (int)GlobalPosition.Y + _collisionChecker.Height / 2);
        set 
        {
            var halfSize = new Vector2(_collisionChecker.Width, _collisionChecker.Height) * 0.5f;
            GlobalPosition =  new Vector2(value.X - halfSize.X, value.Y - halfSize.Y);
        }
    }


    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _audio = GetNode<AudioStreamPlayer>("../Audio");
        _mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
        EventBus.Register<GameStateChangedEvent>(evt =>
        {
            if(evt.NewState == GameState.Running)
            {
                _collisionChecker = new CollisionChecker(_mapGenerator.CollisionMap);
                GridPosition = _mapGenerator.StartPosition((int)SubmarineRadius);
            }
        });

        _game = GetNode<Game>("/root/Game");
    }

    public override void _Process(double delta)
    {
        if (Lazor.IsShooting)
        {
            Shake();
            _sprite.Position = Vector2.Zero;
        }

        if (_game.State == GameState.Running)
            _game.SubmarinePosition = GridPosition;
    }

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
        if (_collisionChecker == null)
            return false;
        if (moveDirection == Vector2.Zero) return false;

        if (_collisionChecker.IsCollision(GlobalPosition + moveDirection * (SubmarineRadius + SubmarineLookahead)))
            return true;
        return false;
    }

    private bool IsStuck()
    {
        return _collisionChecker.IsCollision(GlobalPosition)
               && _collisionChecker.IsCollision(GlobalPosition + Vector2.Right * SubmarineRadius / 2)
               && _collisionChecker.IsCollision(GlobalPosition + Vector2.Down * SubmarineRadius / 2)
               && _collisionChecker.IsCollision(GlobalPosition + Vector2.Left * SubmarineRadius / 2)
               && _collisionChecker.IsCollision(GlobalPosition + Vector2.Up * SubmarineRadius / 2);
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

        if (direction != Vector2.Zero) direction = direction.Normalized();

        return direction;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            if (key.IsReleased() && key.Keycode == Key.R)
                GridPosition = _mapGenerator.StartPosition((int)SubmarineRadius);
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
    private readonly Image _collisionMap;
    public readonly int Width, Height;

    public CollisionChecker(Image collisionMap)
    {
        _collisionMap = collisionMap;
        Width = collisionMap.GetWidth();
        Height = collisionMap.GetHeight();
    }

    public bool IsCollision(Vector2 position)
    {
        var halfSize = new Vector2(Width, Height) * 0.5f;
        if (position.X <= -halfSize.X || position.X >= halfSize.X || position.Y <= -halfSize.Y ||
            position.Y >= halfSize.Y)
            return true;

        var pixel = _collisionMap.GetPixel((int)position.X + Width / 2, (int)position.Y + Height / 2);
        return pixel.R > 0.5f;
    }
}