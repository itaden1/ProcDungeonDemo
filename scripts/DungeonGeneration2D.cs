using Godot;
using System;
using GamePasta.DungeonAlgorythms;


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
        GridDungeon dungeon = new GridDungeon(_mapSize, _segments, _roomCount);



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
        foreach (var n in dungeon.FullMask)
        {
            SetCell((int)n.X + 1, (int)n.Y + 1, -1);
        }
        foreach (var d in dungeon.MainPathDoors)
        {
            SetCell((int)d.Value.X + 1, (int)d.Value.Y + 1, doorTile);
        }
        foreach (var k in dungeon.MainPathKeys)
        {
            SetCell((int)k.Value.X + 1, (int)k.Value.Y + 1, keyTile);
        }
    }
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
        _mapSize = mapSize;
        Generate();
    }
}
