using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class UpscaleStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;
    
    public UpscaleStep(Vector2I mapSize)
    {
        _mapSize = mapSize;
    }
    
    public void Generate(MapGenerationContext ctx)
    {
        var newSize = _mapSize * (int)ctx.UpscaleFactor;
        ctx.WorkingImage.Resize(newSize.X, newSize.Y, Image.Interpolation.Nearest);
    }
}