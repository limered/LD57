using System.Collections.Generic;
using Godot;

namespace valleyfold.Render.Edges;

public partial class EdgeLine : MeshInstance3D
{
    private readonly List<Vector3> _end = new();
    private readonly List<Vector3> _start = new();

    private ImmediateMesh _lineMesh;
    private ShaderMaterial _shaderMaterial;
    private float _width = 0.01f;


    public override void _Ready()
    {
        _lineMesh = new ImmediateMesh();
        Mesh = _lineMesh;

        _shaderMaterial = new ShaderMaterial();
        _shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://src/Render/Edges/edgeline.gdshader");
        MaterialOverride = _shaderMaterial;
    }

    public void LinePositions(Vector3 start, Vector3 end)
    {
        ClearPositions();
        AddPositions(start, end);
    }

    public void AddPositions(Vector3 start, Vector3 end)
    {
        _start.Add(start);
        _end.Add(end);
    }

    public void ClearPositions()
    {
        _start.Clear();
        _end.Clear();
    }

    public void LineWidth(float width)
    {
        _width = width;
    }

    public void LineColor(Color color)
    {
        _shaderMaterial?.SetShaderParameter("line_color", color);
    }

    public void Draw()
    {
        _lineMesh.ClearSurfaces();
        _lineMesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        for (var i = 0; i < _start.Count; ++i)
        {
            var start = _start[i];
            var end = _end[i];

            var direction = (end - start).Normalized();
            var perpendicular = direction.Cross(Vector3.Up).Normalized() * _width;

            var v1 = start - perpendicular;
            var v2 = start + perpendicular;
            var v3 = end - perpendicular;
            var v4 = end + perpendicular;

            _lineMesh.SurfaceAddVertex(v4);
            _lineMesh.SurfaceAddVertex(v2);
            _lineMesh.SurfaceAddVertex(v1);

            _lineMesh.SurfaceAddVertex(v3);
            _lineMesh.SurfaceAddVertex(v4);
            _lineMesh.SurfaceAddVertex(v1);
        }

        _lineMesh.SurfaceEnd();
    }

    public void ClearRendering()
    {
        _lineMesh.ClearSurfaces();
    }
}