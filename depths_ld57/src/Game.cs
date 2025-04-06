using depths_ld57.MapGeneration;
using Godot;

namespace depths_ld57;

public partial class Game : Node2D
{
   
    private MapGenerator _mapGenerator;
    public GameState State { get; private set; }
    
    public override void _Ready()
    {
        _mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
        GoToState(GameState.StartScreen);
        
        _mapGenerator.OnProgressChanged += OnMapGenerationProgressChanged;
    }

    private void OnMapGenerationProgressChanged(float percent)
    {
        GD.Print("Map Generation Percent: " + percent);
    }

    public bool GoToState(GameState newState)
    {
        State = newState;
        switch (State)
        {
            case GameState.StartScreen:
                GD.Print("Start Screen");
                break;
            case GameState.MapGeneration:
                GD.Print("Map Generation");
                _mapGenerator.GenerateMap();
                break;
            case GameState.Running:
                GD.Print("Running");
                break;
            case GameState.Paused:
                GD.Print("Paused");
                break;
            case GameState.GameOver:
                GD.Print("Game Over");
                break;
        }
        return true;
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.N))
        {
            if(State == GameState.GameOver)
            {
                GoToState(GameState.StartScreen);
            }
            else
            {
                GoToState(State + 1);
            }
        }
    }
}