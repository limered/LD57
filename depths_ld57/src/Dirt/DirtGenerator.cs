using System.Collections.Generic;
using System.Linq;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Dirt;

public partial class DirtGenerator : Node
{
    private PackedScene _dirtParticleScene = ResourceLoader
        .Load<PackedScene>("res://scenes/dirt_particle.tscn");

    private readonly List<Texture2D> _dirtParticles = new();
    
    private int _dirtParticleIndex;
    private bool _isGenerating;
    private List<Vector2I> _particlePositions;

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
        var center = new Vector2I(2048, 2048);
        _particlePositions = GetNode<MapGenerator>("/root/LevelGenerator").ParticlePositions;
        _particlePositions = _particlePositions.OrderBy(p => p.DistanceSquaredTo(center)).ToList();
        _isGenerating = true;
    }

    public override void _Process(double delta)
    {
        if (Engine.GetFramesDrawn() % 4 != 0) return;
        if (_isGenerating && _dirtParticleIndex < _particlePositions.Count)
        {
            for (var i = 0; i < 20 && _dirtParticleIndex < _particlePositions.Count; i++)
            {
                var position = _particlePositions[i + _dirtParticleIndex];
                var particle = (DirtParticle)_dirtParticleScene.Instantiate();
                particle.DirtTexture = _dirtParticles[(int)(GD.Randi() % _dirtParticles.Count)];
                particle.GlobalPosition = position - new Vector2I(2048, 2048);
                AddChild(particle);
            }

            _dirtParticleIndex += 20;
        }
    }
}