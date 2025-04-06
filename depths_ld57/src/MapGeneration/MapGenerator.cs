using System;
using System.Collections.Generic;
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
    public Image DirtMap => _context.DirtMap;

    public Action<float> OnProgressChanged;
    
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
            OnProgressChanged?.Invoke(i/(_steps.Count - 1f));
            GD.Print(step.GetType().Name + " done");
        }
        
        EventBus.Emit(new MapGeneratedEvent());
        
        _texture = ImageTexture.CreateFromImage(_context.DirtMap);
        _mapSprite.Texture = _texture;
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.K))
        {
            _texture = ImageTexture.CreateFromImage(_context.WorkingImage);
            _mapSprite.Texture = _texture;
        }
    }
}