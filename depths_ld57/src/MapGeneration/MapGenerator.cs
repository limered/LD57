using System.Collections.Generic;
using depths_ld57.MapGeneration.Steps;
using Godot;

namespace depths_ld57.MapGeneration;

public partial class MapGenerator : Node
{
	[Export] public Vector2I MapSize = new(1024, 1024);
	[Export] public FastNoiseLite Noise;
	[Export] public float WallThreshold = 0.5f;
	[Export] public bool Dilate = true;
	[Export] public bool Erode = false;
	[Export] public int DilateKernelSize = 4; 
	[Export] public int ErodeKernelSize = 4; 
	
	private Sprite2D _mapSprite;
	private Texture2D _texture;
	
	private MapGenerationContext _context;
	private readonly List<IMapGenerationStep> _steps = new();

	public override void _Ready()
	{
		_mapSprite = GetNode<Sprite2D>("MapPreview");


		_context = new MapGenerationContext
		{
			CurrentResultImage = Image.CreateEmpty(MapSize.X, MapSize.Y, false, Image.Format.Rf)
		};
		
		_steps.Add(new BaseNoiseGenerationStep(MapSize, Noise));
		_steps.Add(new ImageProcessingStep(MapSize));
	}
	
	public override void _Process(double delta)
	{
		_context.Dilated = Dilate;
		_context.Eroded = Erode;
		_context.WallThreshold = WallThreshold;
		_context.DilationKernel = DilateKernelSize;
		_context.ErosionKernel = ErodeKernelSize;
		
		if(Input.IsKeyPressed(Key.G))
		{
			foreach (var step in _steps)
			{
				step.Generate(_context);
			}
			_texture = ImageTexture.CreateFromImage(_context.CurrentResultImage);
		}
		
		_mapSprite.Texture = _texture;
	}
}