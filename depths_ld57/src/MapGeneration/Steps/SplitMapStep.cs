using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class SplitMapStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;

    public SplitMapStep(Vector2I mapSize)
    {
        _mapSize = mapSize;
    }

    public void Generate(MapGenerationContext ctx)
    {
        var realMapSize = _mapSize * (int)ctx.UpscaleFactor;
        
        ctx.CollisionMap = Image.CreateEmpty(realMapSize.X, realMapSize.Y, false, Image.Format.Rgbaf);
        ctx.CollisionMap.Fill(Colors.White);
        
        for(var y = 0; y < realMapSize.Y; y++)
        for (var x = 0; x < realMapSize.X; x++)
        {
            var pixel = ctx.WorkingImage.GetPixel(x, y);
            if (pixel.G > 0.5f)
            {
                ctx.CollisionMap.SetPixel(x, y, Colors.Black);
            }
        }

        ctx.DirtMap = Image.CreateEmpty(realMapSize.X, realMapSize.Y, false, Image.Format.Rgbaf);
        ctx.DirtMap.BlitRect(ctx.CollisionMap, new Rect2I(Vector2I.Zero, realMapSize), Vector2I.Zero);
        
        ctx.ColorMap = Image.CreateEmpty(realMapSize.X, realMapSize.Y, false, Image.Format.Rgbaf);
        ctx.ColorMap.Fill(Colors.Black);
        
        for(var y = 0; y < realMapSize.Y; y++)
        for (var x = 0; x < realMapSize.X; x++)
        {
            var pixel = ctx.WorkingImage.GetPixel(x, y);
            if (pixel.R > 0.5f)
            {
                ctx.ColorMap.SetPixel(x, y, Colors.White);
            }
        }
    }
}