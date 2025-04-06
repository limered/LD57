using Godot;

namespace depths_ld57.MapGeneration;

public class MapGenerationContext
{
    public bool Dilated;
    public bool Eroded;
    public int DilationKernel;
    public int ErosionKernel;
    public float WallThreshold;
    public float UpscaleFactor = 8f;
    
    public Image WorkingImage;
    public Image CollisionMap;
    public Image ColorMap;
    public Image DirtMap;
}

public interface IMapGenerationStep
{
    void Generate(MapGenerationContext ctx);
}