using System;
using System.Threading.Tasks;
using Godot;

public class Room : TileMap
{
    [Export] public PackedScene room;

    private bool isAbleToGenerate = true;
    
    [Export] public bool isRight;
    [Export] public bool isStartRoom;

    private Room nRoom;

    public override void _Ready()
    {
        /*if (!isStartRoom)
        {
            if (isRight) GlobalPosition = new Vector2(GlobalPosition.x + 330, GlobalPosition.y);
            else GlobalPosition = new Vector2(GlobalPosition.x - 330, GlobalPosition.y);
        }*/
        
        if (room == null)
        {
            room = ResourceLoader.Load<PackedScene>("res://Rooms/Room" + (GD.Randi() % 2) + ".tscn");
        }
    }

    private void GenerateNextRoom()
    {
        GD.Print(Name);
        GD.Print(room);
        Room nextRoom = (Room) room.Instance();
        //Owner.AddChild(nextRoom);
        GD.Print(nextRoom);
        CallDeferred("add_child", nextRoom);
        nRoom = nextRoom;
        nextRoom.isAbleToGenerate = false;

        nextRoom.isRight = isRight;
        
        Connect("tree_entered", this, nameof(SetTransform));

        GD.Print("generated");

        DelayMethod(nextRoom);
    }

    private void SetTransform()
    {
        GD.Print("isRight " + isRight);
        nRoom.GlobalTransform = GlobalTransform;
        
        if (isRight) nRoom.GlobalPosition = new Vector2(nRoom.GlobalPosition.x + 330, nRoom.GlobalPosition.y);
        else nRoom.GlobalPosition = new Vector2(nRoom.GlobalPosition.x - 330, nRoom.GlobalPosition.y);
    }
    
    private async void DelayMethod(Room nextRoom)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        GD.Print("1 second delay!");
        nextRoom.isAbleToGenerate = true;
    }

    private void _on_detectionZone_body_entered(object body)
    {
        //GD.Print(isAbleToGenerate);
        
        if (!isAbleToGenerate) return;
        
        if (body is PlayerController)
        {
            isAbleToGenerate = false;
            GenerateNextRoom();
        }
        
        GD.Print("bruh2");
    }
}
