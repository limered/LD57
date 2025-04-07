using System.Threading;
using depths_ld57.Dirt;
using depths_ld57.Interface;
using depths_ld57.MapGeneration;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57;

public partial class Game : Node2D
{

    private MapGenerator _mapGenerator;
    public GameState State { get; private set; }
    private Control _startScreen;
    private FaceDive _faceDive;
    private Tutorial _tutorial;
    private AudioStreamPlayer _backgroundAudio;
    private AudioStreamPlayer _dirtPopAudio;
    private AudioStreamPlayer _diveIntoAudio;
    private AudioStreamPlayer _diveOutOfAudio;
    public override void _Ready()
    {
        _startScreen = GetNode<Control>("/root/Main/Camera2D/StartScreen");
        _faceDive = GetNode<FaceDive>("/root/Main/Camera2D/FaceDive");
        _faceDive.OnIntroFinished = () => GetNode<Game>("/root/Game").GoToState(GameState.Tutorial);
        _faceDive.OnOutroFinished = () => GetNode<Game>("/root/Game").GoToState(GameState.StartScreen);
        _tutorial = GetNode<Tutorial>("/root/Main/Camera2D/Tutorial");
        _mapGenerator = GetNode<MapGenerator>("/root/LevelGenerator");
        _generationThread = new Thread(_ =>
        {
            _mapGenerator.GenerateMap();
            _mapGenerated = true;
        });
        
		_backgroundAudio = GetNode<AudioStreamPlayer>("/root/Main/Audio/BackgroundAudio");
		_dirtPopAudio = GetNode<AudioStreamPlayer>("/root/Main/Audio/DirtParticleAudio");
		_diveIntoAudio = GetNode<AudioStreamPlayer>("/root/Main/Audio/FaceDiveIntoAudio");
		_diveOutOfAudio = GetNode<AudioStreamPlayer>("/root/Main/Audio/FaceDiveOutOfAudio");
        
        EventBus.Register<MapGeneratedEvent>(_ => OnMapGenerated());
        EventBus.Register<ParticleExplodedEvent>(_ => {
            _dirtPopAudio.Play();
        });

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
                _startScreen?.SetVisible(true);
                _generationThread.Start();
                break;
            case GameState.IntroAnimation:
                GD.Print("Intro Animation");
                _startScreen?.SetVisible(false);
                _faceDive.PlayIntro();
                break;
            case GameState.Tutorial:
                GD.Print("Tutorial");
                _diveIntoAudio.Play();
                _tutorial.Start();
                break;
            case GameState.MapGeneration:
                GD.Print("Map Generation");
                break;
            case GameState.Running:
                GD.Print("Running");
                _tutorial.Hide();
				_backgroundAudio.Play();
                break;
            case GameState.Paused:
                GD.Print("Paused");
                break;
            case GameState.GameOver:
                GD.Print("Game Over");
                _faceDive.PlayOutro();
                _diveOutOfAudio.Play();
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
            if (State == GameState.GameOver)
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