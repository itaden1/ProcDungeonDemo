using Godot;
using System;
using System.Collections.Generic;
using GamePasta.DungeonAlgorythms;
// using ProcDungeon.Structures;
// using ProcDungeon.Algorythms;
// using ProcDungeon;


public class DungeonGeneration2D : TileMap
{
    [Export]
    private int _mapSize = 40;

    [Export]
    private int _roomCount = 20;
    private enum _tileType { WALL, FLOOR }
    private Random _random = new Random();

    public override void _Ready()
    {
        GD.Print("starting");
        GetParent().GetNode("GUI").Connect("RegenerateDungeon", this, nameof(_onRegenerateButtonPressed));
        Generate();
    }
    private void Generate()
    {
        Clear();
        // Build a random dungeon

        System.Numerics.Vector2 start = new System.Numerics.Vector2(0, (int)_mapSize / 2);

        RandomWalk alg = new RandomWalk(
            new System.Numerics.Vector2(_mapSize, _mapSize),
            start,
            new List<System.Numerics.Vector2>(),
            Direction.EAST,
            0
        );

        SimpleDig alg2 = new SimpleDig(
            new System.Numerics.Vector2(_mapSize, _mapSize),
            start,
            new System.Numerics.Vector2(3, 3),
            10,
            _roomCount
        );

        List<System.Numerics.Vector2> path = alg2.Execute();
        int[] tiles = new int[]{
            TileSet.FindTileByName("wall_1"),
            TileSet.FindTileByName("wall_2"),
            TileSet.FindTileByName("wall_3"),
        };

        for (int i = 0; i < _mapSize - 1; i++)
        {
            for (int j = 0; j < _mapSize - 1; j++)
            {
                var tile = tiles[_random.Next(0, tiles.GetLength(0))];
                SetCell(i, j, tile);

            }
        }
        foreach (System.Numerics.Vector2 vec in path)
        {
            // var tile = tiles[_random.Next(0, tiles.GetLength(0))];
            SetCell((int)vec.X, (int)vec.Y, -1);
        }

        // for(int y = 0; y < myMap.Grid.GetLength(0); y++)
        // {
        // 	for(int x = 0; x < myMap.Grid.GetLongLength(1); x++)
        // 	{
        // 		Tile t = myMap.Grid[y,x];
        // 		if (t.Blocking)
        // 		{
        // 			// place a wall tile
        // 			var tile = tiles[_random.Next(0, tiles.GetLength(0))];
        // 			SetCell(x, y, tile);
        // 		}
        // 	}
        // }
    }
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
        _mapSize = mapSize;
        Generate();
    }
}
