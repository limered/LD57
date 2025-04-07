using System.Runtime.CompilerServices;
using depths_ld57.Score;
using Godot;

namespace depths_ld57.Interface;

public partial class InGameUiNode : Control
{
    [Export] public Label ScoreLabel;
    [Export] public Control ScoreControl;

    private ShaderMaterial _scoreShader;
    
    private Game _game;

    public override void _Ready()
    {
        _game = GetNode<Game>("/root/Game");
        _scoreShader = ScoreControl.Material as ShaderMaterial;
    }

    public override void _Process(double delta)
    {
        if(_game.State == GameState.Running)
        {
            SetVisible(true);
            var percent = 1.0 - ScoreStore.PercentLeft;
            ScoreLabel.Text = (percent * 100).ToString("0.00") + "%";
            _scoreShader.SetShaderParameter("progress", (float)percent);
        }
        else
        {
            SetVisible(false);
        }
    }
}