using Godot;

namespace depths_ld57.Dirt;

public partial class DirtParticle : Area2D
{
    [Export]
    public int Health = 10;

    public Texture2D DirtTexture { set => GetNode<Sprite2D>("Image").Texture = value; }

    public override void _Ready()
    {
        var image = GetNode<Sprite2D>("Image");
        image.Rotation = GD.Randf() * 360f;
        var collider = GetNode<CollisionShape2D>("CollisionShape2D");
        (collider.Shape as CircleShape2D).Radius = (image.Texture.GetWidth() + image.Texture.GetHeight()) / 5f;
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