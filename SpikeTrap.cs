using Godot;

public class SpikeTrap : Node2D
{
    private void _on_Area2D_body_entered(object body)
    {
        if (body is KinematicBody2D)
        {
            if (body is PlayerController)
            {
                PlayerController pc = body as PlayerController;
                pc.TakeDamage();
            }
        }
    }
}
