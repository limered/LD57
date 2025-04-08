using System.Runtime.CompilerServices;
using depths_ld57.MapGeneration;
using depths_ld57.Score;
using depths_ld57.Utils;
using Godot;

namespace depths_ld57.Interface;

public partial class InGameUiNode : Control
{
    [Export] public Label ScoreLabel;
    [Export] public Control ScoreControl;
    [Export] public Control MiniMapControl;

    private ShaderMaterial _scoreShader;
    private ShaderMaterial _miniMapShader;
    
    private Game _game;

    public override void _Ready()
    {
        _game = GetNode<Game>("/root/Game");
        _scoreShader = ScoreControl.Material as ShaderMaterial;
        _miniMapShader = MiniMapControl.Material as ShaderMaterial;
        
        EventBus.Register<MapGeneratedEvent>(_ => AddMapToShader());
    }

    private void AddMapToShader()
    {
        var mapGeneration = GetNode<MapGenerator>("/root/LevelGenerator");
        var texture = ImageTexture.CreateFromImage(mapGeneration.ColorMap);
        _miniMapShader.SetShaderParameter("map", texture);
    }

    public override void _Process(double delta)
    {
        if(_game.State == GameState.Running)
        {
            SetVisible(true);
            var percent = 1.0 - ScoreStore.PercentLeft;
            ScoreLabel.Text = (percent * 100).ToString("0.00") + "%";
            _scoreShader.SetShaderParameter("progress", (float)percent);
            var subPos = _game.SubmarinePosition;
            var shaderPos = new Vector2(subPos.X/4096.0f, subPos.Y/4096.0f);
            _miniMapShader.SetShaderParameter("position", shaderPos);
        }
        else
        {
            SetVisible(false);
        }
    }
}