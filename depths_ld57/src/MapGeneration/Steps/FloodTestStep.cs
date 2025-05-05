using System.Collections.Generic;
using System.Linq;
using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class FloodTestStep : IMapGenerationStep
{
    private readonly Image _floodImage;
    private readonly Vector2I _mapSize;

    public FloodTestStep(Vector2I mapSize)
    {
        _mapSize = mapSize;
        _floodImage = Image.CreateEmpty(_mapSize.X, _mapSize.Y, false, Image.Format.Rgbaf);
    }

    public void Generate(MapGenerationContext ctx)
    {
        CopyIntoFloodImage(ctx);

        var areas = FillAllAreas();
        areas.SortBySize();

        var areaDistanceMap = CalculateAreaToAreaDistances(areas.OrderedAreas, areas.Areas);
        areaDistanceMap = areaDistanceMap.Where(a => a.distance < 5).ToList();

        GenerateDoorsBetweenAreas(areaDistanceMap, areas.Areas, ctx.WorkingImage);

        CopyIntoFloodImage(ctx);

        ctx.MainAreaWallPoints = FillArea(areas.OrderedAreas.First().StartPoint, _floodImage, _mapSize).Item1;

        ctx.WorkingImage = _floodImage;
        ctx.AreaMap = areas;
    }

    private AreaMap FillAllAreas()
    {
        var areas = new AreaMap();
        for (var y = 0; y < _mapSize.Y; y++)
        for (var x = 0; x < _mapSize.X; x++)
        {
            var pixel = _floodImage.GetPixel(x, y);
            if (IsWall(pixel) || IsAlreadyFilled(pixel)) continue;

            var (wallCoords, groundCoords) = FillArea(new Vector2I(x, y), _floodImage, _mapSize);
            if (wallCoords.Count > 10) areas.Add(new Area
            {
                StartPoint = new Vector2I(x, y),
                Walls = wallCoords,
                Ground = groundCoords,
            });
        }

        return areas;
    }

    private static (List<Vector2I>, List<Vector2I>) FillArea(Vector2I position, Image mask, Vector2I size)
    {
        var wallCoords = new List<Vector2I>();
        var groundCoords = new List<Vector2I>();
        var stack = new Stack<Vector2I>();
        stack.Push(position);

        while (stack.Count > 0)
        {
            var pos = stack.Pop();

            if (IsOutOfBounds(pos))
                continue;

            var color = mask.GetPixel(pos.X, pos.Y);
            if (IsWall(color))
            {
                wallCoords.Add(pos);
                continue;
            }

            if (IsAlreadyFilled(color)) continue;

            mask.SetPixel(pos.X, pos.Y, new Color(color.R, 1f, 0));
            groundCoords.Add(pos);

            stack.Push(new Vector2I(pos.X, pos.Y + 1));
            stack.Push(new Vector2I(pos.X, pos.Y - 1));
            stack.Push(new Vector2I(pos.X + 1, pos.Y));
            stack.Push(new Vector2I(pos.X - 1, pos.Y));
        }

        return (wallCoords, groundCoords);

        bool IsOutOfBounds(Vector2I pos)
        {
            return pos.X < 0 || pos.Y < 0 || pos.X >= size.X || pos.Y >= size.Y;
        }
    }

    private static bool IsAlreadyFilled(Color color)
    {
        return color.G > 0.5f;
    }

    private static List<(int areaA, int areaB, int pixelA, int pixelB, int distance)> CalculateAreaToAreaDistances(
        List<Area> areasSortedByCount,
        List<Area> areas)
    {
        var areaMap = new List<(int areaA, int areaB, int pixelA, int pixelB, int distance)>();
        for (var firstIndex = 0; firstIndex < areasSortedByCount.Count; firstIndex++)
        {
            var firstArea = areasSortedByCount[firstIndex];
            for (var secondIndex = 0; secondIndex < areasSortedByCount.Count; secondIndex++)
            {
                var secondArea = areasSortedByCount[secondIndex];
                if (firstArea == secondArea) continue;

                var firstPixelIndex = 0;
                var secondPixelIndex = 0;
                var minDistanceSquared = int.MaxValue;
                for (var i = 0; i < firstArea.Walls.Count; i++)
                for (var j = 0; j < secondArea.Walls.Count; j++)
                {
                    var firstPoint = firstArea.Walls[i];
                    var secondPoint = secondArea.Walls[j];
                    var distance = firstPoint.DistanceSquaredTo(secondPoint);
                    if (distance >= minDistanceSquared) continue;

                    minDistanceSquared = distance;
                    firstPixelIndex = i;
                    secondPixelIndex = j;
                }

                areaMap.Add((
                    areas.IndexOf(firstArea),
                    areas.IndexOf(secondArea),
                    firstPixelIndex,
                    secondPixelIndex,
                    minDistanceSquared));
            }
        }

        return areaMap;
    }

    private void GenerateDoorsBetweenAreas(
        List<(int areaA, int areaB, int pixelA, int pixelB, int distance)> areaMap,
        List<Area> areas,
        Image targetImage)
    {
        foreach (var area in areaMap)
        {
            var pointA = areas[area.areaA].Walls[area.pixelA];
            var pointB = areas[area.areaB].Walls[area.pixelB];
            var center = (pointA + pointB) / 2;
            var radius = area.distance + 2;
            var color = new Color(0, 0, 0);
            for (var y = -radius; y <= radius; y++)
            for (var x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radius * radius) continue;

                var pixelX = center.X + x;
                var pixelY = center.Y + y;
                if (pixelX < 0 || pixelY < 0 || pixelX >= _mapSize.X || pixelY >= _mapSize.Y) continue;

                targetImage.SetPixel(pixelX, pixelY, color);
            }
        }
    }


    private static bool IsWall(Color color)
    {
        return color.R > 0.5f;
    }

    private void CopyIntoFloodImage(MapGenerationContext ctx)
    {
        for (var y = 0; y < _mapSize.Y; y++)
        for (var x = 0; x < _mapSize.X; x++)
        {
            var val = ctx.WorkingImage.GetPixel(x, y).R;
            _floodImage.SetPixel(x, y, new Color(val, 0, 0));
        }
    }
}