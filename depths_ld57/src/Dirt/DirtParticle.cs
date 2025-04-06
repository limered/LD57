using Godot;

namespace depths_ld57.Dirt;

public partial class DirtParticle : Area2D
{
    [Export]
    public int Health = 10;
    
    [Export] public Sprite2D Sprite;
    [Export] public CollisionShape2D Collider;

    public Texture2D DirtTexture { set => Sprite.Texture = value; }

    public override void _Ready()
    {
        Sprite.Rotation = GD.Randf() * 360f;
        ((CircleShape2D)Collider.Shape).Radius = (Sprite.Texture.GetWidth() + Sprite.Texture.GetHeight()) / 5f;
    }

    public void Damage()
    {
        Health--;
        if (Health <= 0)
        {
            QueueFree();
        }
    }
}