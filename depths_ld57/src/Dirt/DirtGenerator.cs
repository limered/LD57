using System.Collections.Generic;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Dirt;

public partial class DirtGenerator : Node
{
    private PackedScene _dirtParticleScene = ResourceLoader
        .Load<PackedScene>("res://scenes/dirt_particle.tscn");

    private readonly List<Texture2D> _dirtParticles = new();
    public override void _Ready()
    {
        EventBus.Register<MapGeneratedEvent>(_ => GenerateDirt());
        
        for(var i = 1; i <= 12; i++)
        {
            for (var j = 1; j <= 3; j++)
            {
                var dirtTexture = ResourceLoader
                    .Load<Texture2D>($"res://graphics/microplastic/microplastic_{i}_{j}.png");
                _dirtParticles.Add(dirtTexture);
            }
        }
    }

    private void GenerateDirt()
    {
        var particlePositions = GetNode<MapGenerator>("/root/LevelGenerator").ParticlePositions;
        foreach (var position in particlePositions)
        {
            var particle = (DirtParticle)_dirtParticleScene.Instantiate();
            particle.DirtTexture = _dirtParticles[(int)(GD.Randi() % _dirtParticles.Count)];
            particle.GlobalPosition = position - new Vector2I(2048, 2048);
            AddChild(particle);
        }
    }
}