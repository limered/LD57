using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class BaseNoiseGenerationStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;

    private readonly NoiseTexture2D _noiseTexture;
    
    public BaseNoiseGenerationStep(Vector2I mapSize, FastNoiseLite noise)
    {
        _mapSize = mapSize;

        _noiseTexture = new NoiseTexture2D();
        _noiseTexture.Noise = noise;
        _noiseTexture.Width = _mapSize.X;
        _noiseTexture.Height =_mapSize.Y;
        _noiseTexture.Normalize = true;
    }

    public void Generate(MapGenerationContext ctx)
    {
        var noiseImage = _noiseTexture.GetImage();
        if (noiseImage is null) return;
		
        for (var x = 0; x < _mapSize.X; x++)
        {
            for (var y = 0; y < _mapSize.Y; y++)
            {
                var pixelColor = noiseImage.GetPixel(x, y);
                var brightness = pixelColor.R;

                ctx.CurrentResultImage
                    .SetPixel(x, y, brightness > ctx.WallThreshold ? Colors.White : Colors.Black);
            }
        }
    }
}