using Godot;

public class Room : TileMap
{
    [Export] 
    public PackedScene room;

    private bool isGeneratedNewRoom;
    
    private bool isRightOrLeftPlayerComing;
    private bool isRightPlayerComing;
    
    public override void _Ready()
    {
        GD.Randomize();
    }

    private void GenerateNextRoom(bool isRight)
    {
        Room nextRoom = room.Instance() as Room;
        Owner.AddChild(nextRoom);
        nextRoom.GlobalTransform = GlobalTransform;

        nextRoom.isRightOrLeftPlayerComing = true;
        
        if (isRight)
        {
            nextRoom.GlobalPosition = new Vector2(nextRoom.GlobalPosition.x + 304, nextRoom.GlobalPosition.y);
            nextRoom.isRightPlayerComing = true;
        }
        else
        {
            nextRoom.GlobalPosition = new Vector2(nextRoom.GlobalPosition.x - 304, nextRoom.GlobalPosition.y);
            nextRoom.isRightPlayerComing = false;
        }
        
        GD.Print("generated");
    }

    private void _on_detectionZoneLeft_body_entered(object body)
    {
        if (body is PlayerController)
        {
            if (isGeneratedNewRoom) return;
            if (isRightOrLeftPlayerComing && !isRightPlayerComing) return;
            
            GenerateNextRoom(false);
            isGeneratedNewRoom = true;
        }
    }

    private void _on_detectionZoneRight_body_entered(object body)
    {
        if (body is PlayerController)
        {
            if (isGeneratedNewRoom) return;
            if (isRightOrLeftPlayerComing && isRightPlayerComing) return;
            
            GenerateNextRoom(true);
            isGeneratedNewRoom = true;
        } 
    }
}
