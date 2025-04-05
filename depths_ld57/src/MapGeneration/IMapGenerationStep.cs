using Godot;

namespace depths_ld57.MapGeneration;

public class MapGenerationContext
{
    public Image CurrentResultImage;
    public bool Dilated;
    public bool Eroded;
    public int DilationKernel;
    public int ErosionKernel;
    public float WallThreshold;
}

public interface IMapGenerationStep
{
    void Generate(MapGenerationContext ctx);
}