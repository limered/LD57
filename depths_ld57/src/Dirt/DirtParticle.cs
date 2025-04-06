using Godot;

namespace depths_ld57.Dirt;

public partial class DirtParticle : Sprite2D
{
    public override void _Ready()
    {
        var dirtType = GD.Randi() % 12;
        var dirtColor = GD.Randi() % 3;
        var dirtTexture = ResourceLoader
            .Load<Texture2D>($"res://graphics/microplastic/microplastic_{dirtType}_{dirtColor}.png");
        Texture = dirtTexture;
    }
}