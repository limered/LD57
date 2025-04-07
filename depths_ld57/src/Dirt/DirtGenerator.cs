using System.Collections.Generic;
using System.Linq;
using System.Threading;
using depths_ld57.MapGeneration;
using depths_ld57.Score;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Dirt;

public partial class DirtGenerator : Node
{
    [Export] public float LoadThreshold = 50; 
    
    private PackedScene _dirtParticleScene = ResourceLoader
        .Load<PackedScene>("res://scenes/dirt_particle.tscn");
    private Camera2D _camera;

    private readonly List<Texture2D> _dirtParticles = new();
    
    private int _dirtParticleIndex;
    private bool _isGenerating;
    private TailRemoveList<Vector2I> _particlePositions;
    private Vector2I _center;

    private int _batchSize = 20;

    public override void _Ready()
    {
        EventBus.Register<MapGeneratedEvent>(_ => PrepareDirtGeneration());
        
        for(var i = 1; i <= 12; i++)
        {
            for (var j = 1; j <= 3; j++)
            {
                var dirtTexture = ResourceLoader
                    .Load<Texture2D>($"res://graphics/microplastic/microplastic_{i}_{j}.png");
                _dirtParticles.Add(dirtTexture);
            }
        }
        
        _camera = GetNode<Camera2D>("/root/Main/Camera2D");
    }

    private void PrepareDirtGeneration()
    {
        _center = new Vector2I(2048, 2048);
        var dirtPositions = GetNode<MapGenerator>("/root/LevelGenerator").ParticlePositions;
        _particlePositions = new TailRemoveList<Vector2I>(dirtPositions
            .OrderBy(p => p.DistanceSquaredTo(_center))
            .ToList());
        _isGenerating = true;
        ScoreStore.DirtParticlesMax = dirtPositions.Count;
    }

    
    public override void _Process(double delta)
    {
        ScoreStore.DirtParticlesLeft = (_particlePositions is null) ? 
            0 : 
            _particlePositions.Count + GetChildCount();
        
        if (!_isGenerating || _particlePositions is null || _particlePositions.Count == 0) return;
        var rect = _camera.GetViewportRect();
        var camPos = _camera.GlobalPosition;
        rect.Position = camPos - rect.End / 2;
        rect = rect.Grow(LoadThreshold);

        var c = 0;
        for (var i = _particlePositions.Count - 1; i >= 0; i--)
        {
            var pos = _particlePositions[i];
            if (rect.HasPoint(pos - _center))
            {
                
                var particle = (DirtParticle)_dirtParticleScene.Instantiate();
                particle.DirtTexture = _dirtParticles[(int)(GD.Randi() % _dirtParticles.Count)];
                particle.GlobalPosition = pos - _center;
                particle.SetProcess(false);
                AddChild(particle);
                _particlePositions.Remove(i);
                if (c++ >= _batchSize) break;
            }
        }
    }
}