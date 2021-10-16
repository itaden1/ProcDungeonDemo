using Godot;
using System;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;

public class GenerationController : Spatial
{
    [Export]
	public int MapSize = 10;

	[Export]
	public int RoomCount = 2;

    public override void _Ready()
    {
        Generate();
    }

    public void Generate()
    {
        GD.Print("generating");
		var alg = new BSPDungeonAlgorythm(RoomCount);
		var myMap = new DungeonGrid(MapSize, alg);
		myMap.GenerateLayout();

        DungeonGeneration3D dungeon3D = GetNode<DungeonGeneration3D>("Dungeon");
        Map dungeonMap = GetNode<Map>("CanvasLayer/Map");

        dungeon3D.Generate(myMap);
        dungeonMap.Generate(myMap.Grid);
        dungeonMap.Update();
    }
}
