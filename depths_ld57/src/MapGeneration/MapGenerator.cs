using System.Collections.Generic;
using System.Linq;
using depths_ld57.MapGeneration.Steps;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.MapGeneration;

public partial class MapGenerator : Node
{
    private readonly List<IMapGenerationStep> _steps = new();

    private MapGenerationContext _context;

    private Sprite2D _mapSprite;
    private Texture2D _texture;
    [Export] public bool Dilate = true;
    [Export] public int DilateKernelSize = 2;
    [Export] public bool Erode = true;
    [Export] public int ErodeKernelSize = 6;
    [Export] public Vector2I MapSize = new(1024, 1024);
    [Export] public FastNoiseLite Noise = new();
    [Export] public float UpscaleFactor = 8f;
    [Export] public float WallThreshold = 0.67f;

    public Image CollisionMap => _context.CollisionMap;
    public Image ColorMap => _context.ColorMap;
    private Image DirtMap => _context.DirtMap;
    public List<Vector2I> ParticlePositions => _context.DirtParticles;

    public Vector2I StartPosition(int radius)
    {
        GD.Print(_context.AreaMap.OrderedAreas.Count);
        var largestArea = _context.AreaMap.OrderedAreas.First();
        var randomWallPoint = largestArea.Walls[(int)(GD.Randi() % largestArea.Walls.Count)] * (int)UpscaleFactor;
        var point = randomWallPoint + new Vector2I(radius, 0);
        var tries = 0;
        while (!CheckArea(point))
        {
            point = randomWallPoint + new Vector2I((int)(GD.Randi() % radius * 2), (int)(GD.Randi() % radius * 2));
            if (tries++ % 10 == 0)
                randomWallPoint = largestArea.Walls[(int)(GD.Randi() % largestArea.Walls.Count)] * (int)UpscaleFactor;
        }

        return point;

        bool CheckArea(Vector2I coordinate)
        {
            for (var x = -radius; x < radius; x++)
            for (var y = -radius; y < radius; y++)
            {
                var p = coordinate + new Vector2I(x, y);
                var pixel = _context.CollisionMap.GetPixel(p.X, p.Y);
                if (pixel.R > 0.5f) return false;
            }

            return true;
        }
    }

    public override void _Ready()
    {
        _mapSprite = GetNode<Sprite2D>("WorldView");

        Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        Noise.Frequency = 0.015f;
        Noise.Seed = GD.RandRange(0, 10000);

        Noise.SetFractalType(FastNoiseLite.FractalTypeEnum.Ridged);
        Noise.FractalOctaves = 2;
        Noise.FractalLacunarity = 2.0f;
        Noise.FractalGain = 0.6f;

        _context = new MapGenerationContext
        {
            WorkingImage = Image.CreateEmpty(MapSize.X, MapSize.Y, false, Image.Format.Rgbaf)
        };

        _steps.Add(new BaseNoiseGenerationStep(MapSize, Noise));
        _steps.Add(new ImageProcessingStep(MapSize));
        _steps.Add(new BrainFormStep(MapSize));
        _steps.Add(new FloodTestStep(MapSize));
        _steps.Add(new UpscaleStep(MapSize));
        _steps.Add(new SplitMapStep(MapSize));
        _steps.Add(new AddDirtStep(MapSize));

        EventBus.Register<MapGeneratedEvent>(_ =>
        {
            _texture = ImageTexture.CreateFromImage(_context.ColorMap);
            _mapSprite.Texture = _texture;
        });
    }

    public void GenerateMap()
    {
        _context.Dilated = Dilate;
        _context.Eroded = Erode;
        _context.WallThreshold = WallThreshold;
        _context.DilationKernel = DilateKernelSize;
        _context.ErosionKernel = ErodeKernelSize;
        _context.UpscaleFactor = UpscaleFactor;

        for (var i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            step.Generate(_context);
            GD.Print(step.GetType().Name + " done");
        }

        GD.Print("Map Generated!");
    }
}