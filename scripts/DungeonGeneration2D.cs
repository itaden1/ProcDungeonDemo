using Godot;
using System;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;

public class DungeonGeneration2D : TileMap
{
	[Export]
	private int _mapSize = 40;

	[Export]
	private int _roomCount = 20;
	private enum _tileType {WALL, FLOOR}
	private Random _random = new Random();

	public override void _Ready()
	{
		GetParent().GetNode("GUI").Connect("RegenerateDungeon", this, nameof(_onRegenerateButtonPressed));
		Generate();
	}
	private void Generate()
	{
		Clear();
		// Build a random dungeon
		var alg = new BSPDungeonAlgorythm(_roomCount);
		var myMap = new DungeonGrid(_mapSize, alg);
		myMap.GenerateLayout();


		int[] tiles = new int[]{
			TileSet.FindTileByName("wall_1"),
			TileSet.FindTileByName("wall_2"),
			TileSet.FindTileByName("wall_3"),
		};

		for(int y = 0; y < myMap.Grid.GetLength(0); y++)
		{
			for(int x = 0; x < myMap.Grid.GetLongLength(1); x++)
			{
				Tile t = myMap.Grid[y,x];
				if (t.Blocking)
				{
					// place a wall tile
					var tile = tiles[_random.Next(0, tiles.GetLength(0))];
					SetCell(x, y, tile);
				}
			}
		}
	}
	private void _onRegenerateButtonPressed(int rooms, int mapSize)
	{
		_roomCount = rooms;
		_mapSize = mapSize;
		Generate();
	}
}
