using System.Runtime.CompilerServices;
using depths_ld57.Score;
using Godot;

namespace depths_ld57.Interface;

public partial class InGameUiNode : Control
{
    [Export] public Label ScoreLabel;

    private Game _game;

    public override void _Ready()
    {
        _game = GetNode<Game>("/root/Game");
    }

    public override void _Process(double delta)
    {
        if(_game.State == GameState.Running)
        {
            SetVisible(true);
            ScoreLabel.Text = "Percent Done: " + ((1.0 - ScoreStore.PercentLeft) * 100).ToString("0.00") + "%";
        }
        else
        {
            SetVisible(false);
            ScoreLabel.Text = "";
        }
    }
}