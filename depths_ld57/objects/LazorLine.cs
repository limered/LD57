using Godot;

public partial class LazorLine : MeshInstance2D
{
    [Export]
    public Color LineColor
    {
        get;
        set;
    }

    [Export]
    public float LineWidth = 0.01f;

    [Export]
    public Vector2 Start { get; set; }

    [Export]
    public Vector2 End { get; set; }

    private readonly ImmediateMesh _lineMesh = new();


    public override void _Ready()
    {
        Mesh = _lineMesh;
        // Material = shaderMaterial;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        DrawLine();
    }

    public void DrawLine()
    {
        if (Start == End)
            return;

        _lineMesh.ClearSurfaces();
        _lineMesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        var direction = (End - Start).Normalized();
        var perpendicular = direction.Rotated(Mathf.DegToRad(90)) * LineWidth;

        var v1 = Start - perpendicular;
        var v2 = Start + perpendicular;
        var v3 = End - perpendicular;
        var v4 = End + perpendicular;

        _lineMesh.SurfaceAddVertex2D(v4);
        _lineMesh.SurfaceAddVertex2D(v2);
        _lineMesh.SurfaceAddVertex2D(v1);

        _lineMesh.SurfaceAddVertex2D(v3);
        _lineMesh.SurfaceAddVertex2D(v4);
        _lineMesh.SurfaceAddVertex2D(v1);

        _lineMesh.SurfaceEnd();
    }
}