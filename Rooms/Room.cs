using System;
using System.Threading.Tasks;
using Godot;

public class Room : TileMap
{
    [Export] public PackedScene room;

    private bool isAbleToGenerate = true;

    private Room nRoom;

    public override void _Ready()
    {
        if (room == null)
        {
            room = ResourceLoader.Load<PackedScene>("res://Rooms/Room" + (GD.Randi() % 4) + ".tscn");
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

        Connect("tree_entered", this, nameof(SetTransform));

        GD.Print("generated");

        DelayMethod(nextRoom);
    }

    private void SetTransform()
    {
        nRoom.GlobalTransform = GlobalTransform;
        
        nRoom.GlobalPosition = new Vector2(nRoom.GlobalPosition.x + 752, nRoom.GlobalPosition.y);
    }
    
    private async void DelayMethod(Room nextRoom)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        GD.Print("delay generate");
        nextRoom.isAbleToGenerate = true;
    }

    private void _on_detectionZone_body_entered(object body)
    {
        //GD.Print(isAbleToGenerate);
        
        if (!isAbleToGenerate) return;
        
        if (body is PlayerController pc)
        {
            isAbleToGenerate = false;
            GenerateNextRoom();

            pc.AddScore();
        }
        
        GD.Print("bruh2");
    }
}
