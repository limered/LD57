using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Dirt;

public partial class DirtGenerator : Node
{
    private PackedScene _dirtParticleScene = ResourceLoader
        .Load<PackedScene>("res://scenes/dirt_particle.tscn");
        
    public override void _Ready()
    {
        EventBus.Register<MapGeneratedEvent>(_ => GenerateDirt());
    }

    private void GenerateDirt()
    {
        var particlePositions = GetNode<MapGenerator>("/root/LevelGenerator").ParticlePositions;
        foreach (var position in particlePositions)
        {
            var particle = (DirtParticle)_dirtParticleScene.Instantiate();
            particle.Position = position;
            AddChild(particle);
        }
    }
}