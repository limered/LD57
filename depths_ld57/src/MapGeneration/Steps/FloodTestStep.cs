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
        // TODO: Maybe use Area Information later
        var areasSortedBySize = areas.OrderByDescending(pair => pair.pixels.Count).ToList();

        var areaMap = CalculateAreaToAreaDistances(areasSortedBySize, areas);
        areaMap = areaMap.Where(a => a.distance < 5).ToList();

        GenerateDoorsBetweenAreas(areaMap, areas, ctx.WorkingImage);

        CopyIntoFloodImage(ctx);

        ColorArea(areasSortedBySize.First().start, _floodImage, _mapSize);

        ctx.WorkingImage = _floodImage;
    }

    private List<(Vector2I start, List<Vector2I> pixels)> FillAllAreas()
    {
        var areas = new List<(Vector2I start, List<Vector2I> pixels)>();
        for (var y = 0; y < _mapSize.Y; y++)
        for (var x = 0; x < _mapSize.X; x++)
        {
            var pixel = _floodImage.GetPixel(x, y);
            if (pixel.R > 0.5f) continue;

            var coords = ColorArea(new Vector2I(x, y), _floodImage, _mapSize);
            if (coords.Count > 10) areas.Add((new Vector2I(x, y), coords));
        }

        return areas;
    }

    private static List<(int areaA, int areaB, int pixelA, int pixelB, int distance)> CalculateAreaToAreaDistances(
        List<(Vector2I start, List<Vector2I> pixels)> areasSortedByCount,
        List<(Vector2I start, List<Vector2I> pixels)> areas)
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
                for (var i = 0; i < firstArea.pixels.Count; i++)
                for (var j = 0; j < secondArea.pixels.Count; j++)
                {
                    var firstPoint = firstArea.pixels[i];
                    var secondPoint = secondArea.pixels[j];
                    var distance = firstPoint.DistanceSquaredTo(secondPoint);
                    if (distance < minDistanceSquared)
                    {
                        minDistanceSquared = distance;
                        firstPixelIndex = i;
                        secondPixelIndex = j;
                    }
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
        List<(Vector2I start, List<Vector2I> pixels)> areas, 
        Image targetImage)
    {
        foreach (var area in areaMap)
        {
            var pointA = areas[area.areaA].pixels[area.pixelA];
            var pointB = areas[area.areaB].pixels[area.pixelB];
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

    private static List<Vector2I> ColorArea(Vector2I position, Image mask, Vector2I size)
    {
        var coordinates = new List<Vector2I>();
        var stack = new Stack<Vector2I>();
        stack.Push(position);

        while (stack.Count > 0)
        {
            var pos = stack.Pop();

            if (pos.X < 0 || pos.Y < 0 || pos.X >= size.X || pos.Y >= size.Y)
                continue;

            var color = mask.GetPixel(pos.X, pos.Y);

            if (color.R > 0.5f)
            {
                coordinates.Add(pos);
                continue;
            }

            if (color.G > 0.5f) continue;

            mask.SetPixel(pos.X, pos.Y, new Color(color.R, 1f, 0));

            stack.Push(new Vector2I(pos.X, pos.Y + 1));
            stack.Push(new Vector2I(pos.X, pos.Y - 1));
            stack.Push(new Vector2I(pos.X + 1, pos.Y));
            stack.Push(new Vector2I(pos.X - 1, pos.Y));
        }

        return coordinates;
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