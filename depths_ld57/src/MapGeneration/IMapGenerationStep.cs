using Godot;

namespace depths_ld57.MapGeneration;

public class MapGenerationContext
{
    public bool Dilated;
    public bool Eroded;
    public int DilationKernel;
    public int ErosionKernel;
    public float WallThreshold;
    
    public Image WorkingImage;
}

public interface IMapGenerationStep
{
    void Generate(MapGenerationContext ctx);
}