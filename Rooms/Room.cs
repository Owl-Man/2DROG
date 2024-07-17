using Godot;

public class Room : TileMap
{
    [Export] 
    public PackedScene room;
    
    private bool isRightOrLeftPlayerComing;
    private bool isRightPlayerComing;
    
    /*public override void _Ready()
    {
    }*/

    private void GenerateNextRoom(bool isRight)
    {
        Room nextRoom = room.Instance() as Room;
        Owner.AddChild(nextRoom);
        nextRoom.GlobalTransform = GlobalTransform;

        nextRoom.isRightOrLeftPlayerComing = true;
        
        if (isRight)
        {
            nextRoom.GlobalPosition = new Vector2(nextRoom.GlobalPosition.x + 330, nextRoom.GlobalPosition.y);
            nextRoom.isRightPlayerComing = true;
            nextRoom.GetNode<Position2D>("doorway1").QueueFree();
        }
        else
        {
            nextRoom.GlobalPosition = new Vector2(nextRoom.GlobalPosition.x - 330, nextRoom.GlobalPosition.y);
            nextRoom.isRightPlayerComing = false;
            nextRoom.GetNode<Position2D>("doorway2").QueueFree();
        }
        
        GD.Print("generated");
    }

    private void _on_detectionZoneLeft_body_entered(object body)
    {
        if (body is PlayerController)
        {
            if (isRightOrLeftPlayerComing && !isRightPlayerComing) return;
            
            GenerateNextRoom(false);
        }
    }

    private void _on_detectionZoneRight_body_entered(object body)
    {
        if (body is PlayerController)
        {
            if (isRightOrLeftPlayerComing && isRightPlayerComing) return;
            
            GenerateNextRoom(true);
        } 
    }
}
