using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class BrainFormStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;

    public BrainFormStep(Vector2I mapSize)
    {
        _mapSize = mapSize;
    }

    public void Generate(MapGenerationContext ctx)
    {
        var halfSize = _mapSize / 2;
        var eightSize = _mapSize / 8;
        var sixthSize = _mapSize / 16;
        
        var leftHalf = halfSize.X - eightSize.X - sixthSize.X;
        var rightHalf = halfSize.X + eightSize.X + sixthSize.X;

        var mask = Image.CreateEmpty(_mapSize.X, _mapSize.Y, false, Image.Format.Rf);
        mask.Fill(new Color(1, 1, 1, 1));

        DrawMask(new Vector2I(leftHalf, halfSize.Y), _mapSize, mask);
        DrawMask(new Vector2I(rightHalf, halfSize.Y), _mapSize, mask);
        
        ApplyMAsk(_mapSize, mask, ctx.CurrentResultImage);
    }
    
    private static void DrawMask(Vector2I position, Vector2I size, Image mask)
    {
        var majorAxis = size.X * 0.3f;
        var minorAxis = size.Y * 0.5f;
        
        for (var y = 0; y < size.Y; y++)
        {
            for (var x = 0; x < size.X; x++)
            {
                var dx = (x - position.X) / majorAxis;
                var dy = (y - position.Y) / minorAxis;

                var perturbation = Mathf.Sin(dx * Mathf.Pi * 4) * Mathf.Sin(dy * Mathf.Pi * 4) * 0.1f;

                if ((dx * dx + dy * dy) <= (1 + perturbation))
                {
                    mask.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
    }
    
    private static void ApplyMAsk(Vector2I size, Image mask, Image targetImage)
    {
        for (var y = 0; y < size.Y; y++)
        {
            for (var x = 0; x < size.X; x++)
            {
                var pixel = mask.GetPixel(x, y);
                if (pixel.R >= 0.5f)
                {
                    targetImage.SetPixel(x, y, pixel);
                }
            }
        }
    }
}