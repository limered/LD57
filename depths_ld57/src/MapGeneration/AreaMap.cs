using System.Collections.Generic;
using System.Linq;

namespace depths_ld57.MapGeneration;

public class AreaMap
{
    public List<Area> Areas { get; } = new();
    public List<Area> OrderedAreas { get; private set; } = new();

    public void Add(Area area)
    {
        Areas.Add(area);
    }

    public void SortBySize()
    {
        OrderedAreas = Areas.OrderByDescending(area => area.Walls.Count).ToList();
    }
}