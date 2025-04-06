using Godot;

namespace depths_ld57.Dirt;

public partial class DirtParticle : Area2D
{
    [Export]
    public int Health = 10;
    
    [Export] public Sprite2D Sprite;
    [Export] public CollisionShape2D Collider;

    public Texture2D DirtTexture { set => Sprite.Texture = value; }
    private CpuParticles2D explodeParticles;

    private double disposeAfter = 2f;

    public override void _Ready()
    {
        Sprite.Rotation = GD.Randf() * 360f;
        ((CircleShape2D)Collider.Shape).Radius = (Sprite.Texture.GetWidth() + Sprite.Texture.GetHeight()) / 5f;
        (collider.Shape as CircleShape2D).Radius = (image.Texture.GetWidth() + image.Texture.GetHeight()) / 5f;
        explodeParticles = GetNode<CpuParticles2D>("CPUParticles2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Health <= 0 && disposeAfter > 0)
        {
            disposeAfter -= delta;

            if (disposeAfter <= 0)
            {
                QueueFree();
            }
        }

    }


    public void Damage()
    {
        if (Health <= 0) return;

        Health--;
        if (Health <= 0)
        {
            explodeParticles.Emitting = true;
            GetNode<Sprite2D>("Image")?.QueueFree();
        }
    }
}