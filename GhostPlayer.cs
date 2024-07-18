using Godot;

public class GhostPlayer : Node2D
{
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeOut");
    }

    public void Destroy()
    {
        QueueFree();
    }
}
