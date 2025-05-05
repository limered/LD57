using System;
using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class CreateLevelMapStep : IMapGenerationStep
{
    private readonly Vector2I _size;

    private readonly Color[] _levelColors = {
        new Color(0, 0, 0, 0), // biggest area (spawn)
        new Color(0, 0, 0, 1), // only moving enemies
        new Color(0, 0, 1, 0), // wall enemies
        new Color(0, 0, 1, 1), // combination
        new Color(0, 1, 0, 0), // end boss
        new Color(0, 1, 0, 1), // small boss
        new Color(0, 1, 1, 0),
        new Color(0, 1, 1, 1), 
    };

    public CreateLevelMapStep(Vector2I size)
    {
        _size = size;
    }
    
    public void Generate(MapGenerationContext ctx)
    {
        ctx.Levels = Image.CreateEmpty(_size.X, _size.Y, false, Image.Format.Rgbah);

        var levelCounter = 1u;
        foreach (var area in ctx.AreaMap.OrderedAreas)
        {
            for (var i = 0; i < area.Ground.Count; i++)
            {
                var ground = area.Ground[i];
                var thisLevel = Math.Max(levelCounter, 5);
                ctx.Levels.SetPixel(ground.X, ground.Y, _levelColors[thisLevel]);
            }
            
            levelCounter++;
        }
    }
}