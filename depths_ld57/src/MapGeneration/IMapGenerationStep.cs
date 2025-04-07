using System.Collections.Generic;
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

    public int DirtParticleRadius = 10;
    public float DirtParticleSpeed = 1.5f;
    public float DirtParticleCount = 7500;
    public List<Vector2I> FutureDirtPositions = new();
    public List<Vector2I> DirtParticles;
}

public interface IMapGenerationStep
{
    void Generate(MapGenerationContext ctx);
}