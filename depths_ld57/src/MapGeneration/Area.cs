using System.Collections.Generic;
using Godot;

namespace depths_ld57.MapGeneration;

public class Area
{
    public Vector2I StartPoint { get; set; }
    public List<Vector2I> Walls = new();
}