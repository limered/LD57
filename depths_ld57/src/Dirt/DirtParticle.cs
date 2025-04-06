using Godot;

namespace depths_ld57.Dirt;

public partial class DirtParticle : Node2D
{
    public Texture2D DirtTexture {set => GetNode<Sprite2D>("Image").Texture = value;}

    public override void _Ready()
    {
        GetNode<Sprite2D>("Image").Rotation = GD.Randf() * 360f;
    }
}