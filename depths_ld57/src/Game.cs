using System.Threading;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57;

public partial class Game : Node2D
{
   
    private MapGenerator _mapGenerator;
    public GameState State { get; private set; }
    
    public override void _Ready()
    {
        _mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
        _generationThread = new Thread(_ =>
        {
            _mapGenerator.GenerateMap();
            _mapGenerated = true;
        });
        
        EventBus.Register<MapGeneratedEvent>(_ => OnMapGenerated());
        GoToState(GameState.StartScreen);
    }

    private bool _mapGenerated;
    private Thread _generationThread;

    private void OnMapGenerated()
    {
        GoToState(GameState.Running);
    }

    public bool GoToState(GameState newState)
    {
        if (State == newState) return false;
        State = newState;
        switch (State)
        {
            case GameState.StartScreen:
                GD.Print("Start Screen");
                var ui1 = GetNode<Control>("/root/Node2D2/Interface");
                if (ui1 is not null)
                {
                    ui1.SetVisible(true);
                }
                _generationThread.Start();
                break;
            case GameState.MapGeneration:
                GD.Print("Map Generation");
                break;
            case GameState.Running:
                var ui2 = GetNode<Control>("/root/Node2D2/Interface");
                if (ui2 is not null)
                {
                    ui2.SetVisible(false);
                }
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
        if (State == GameState.MapGeneration && _mapGenerated)
        {
            EventBus.Emit(new MapGeneratedEvent());
        }
        
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