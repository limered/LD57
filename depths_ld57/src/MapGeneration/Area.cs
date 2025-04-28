using System.Collections.Generic;
using Godot;

namespace depths_ld57.MapGeneration;

public class Area
{
    public Vector2I StartPoint { get; init; }
    public List<Vector2I> Walls = new();
    public Image AreaGround;
}