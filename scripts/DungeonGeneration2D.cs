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
    private int _mapSize = 60;

    [Export]
    private int _roomCount = 4;

    [Export]
    private int _segments = 5;

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
        GridDungeon dungeonAlg = new GridDungeon(_mapSize, _segments, _roomCount);
        Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> segmentPaths = dungeonAlg.Generate();



        int[] tiles = new int[]{
            TileSet.FindTileByName("wall_1"),
            TileSet.FindTileByName("wall_2"),
            TileSet.FindTileByName("wall_3"),
        };

        int doorTile = TileSet.FindTileByName("door");
        int keyTile = TileSet.FindTileByName("key");

        for (int i = 0; i < _mapSize + 3; i++)
        {
            for (int j = 0; j < _mapSize + 3; j++)
            {
                var tile = tiles[_random.Next(0, tiles.GetLength(0))];
                SetCell(i, j, tile);

            }
        }

        // for each path in each segment draw it to the screen
        foreach (var v in segmentPaths)
        {

            foreach (var p in v.Value)
            {
                SetCell((int)p.X + 1, (int)p.Y + 1, -1);
            }
            if (dungeonAlg.MainPathDoors.ContainsKey(v.Key))
            {
                var d = dungeonAlg.MainPathDoors[v.Key];
                SetCell((int)d.X + 1, (int)d.Y + 1, doorTile);
            }
            if (dungeonAlg.MainPathKeys.ContainsKey(v.Key))
            {
                var k = dungeonAlg.MainPathKeys[v.Key];
                SetCell((int)k.X + 1, (int)k.Y + 1, keyTile);
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
