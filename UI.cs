using Godot;

public class UI : Control
{
    public override void _Ready()
    {
        _on_Player_UpdateScore(0);
        GetTree().Root.GetNode("Level").GetNode<PlayerController>("Player").uScore += _on_Player_UpdateScore;
    }

    private void _on_Player_UpdateScore(int score)
    {
        GetNode<Panel>("Panel").GetNode<Label>("Label").Text = "exp - " + score;
    }
}
