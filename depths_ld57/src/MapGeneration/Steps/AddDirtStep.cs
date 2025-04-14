using System.Collections.Generic;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class AddDirtStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;

    public AddDirtStep(Vector2I mapSize)
    {
        _mapSize = mapSize;
    }

    public void Generate(MapGenerationContext ctx)
    {
        var realSize = _mapSize * (int)ctx.UpscaleFactor;
            
        ctx.DirtParticles = new List<Vector2I>((int)ctx.DirtParticleCount);
        for (var i = 0; i < ctx.DirtParticleCount; i++)
        {
            var randomIndex = GD.Randi() % ctx.MainAreaWallPoints.Count;
            var randomPosition = ctx.MainAreaWallPoints[(int)randomIndex];
            ctx.DirtParticles.Add(randomPosition * (int)ctx.UpscaleFactor);
        }
        
        foreach (var particle in ctx.DirtParticles)
        {
            DrawCircle(particle, ctx.DirtParticleRadius, ctx.DirtMap, new Color("#7f7f00"));
        }
    }

    private static void DrawCircle(Vector2I center, int radius, Image image, Color color)
    {
        for (var y = -radius; y <= radius; y++)
        for (var x = -radius; x <= radius; x++)
        {
            if (x * x + y * y > radius * radius) continue;
            var pixel = center + new Vector2I(x, y);
            if (pixel.X < 0 || pixel.X >= image.GetWidth() || pixel.Y < 0 || pixel.Y >= image.GetHeight()) continue;
            image.SetPixel(pixel.X, pixel.Y, color);
        }
    }
}