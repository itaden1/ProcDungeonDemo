using Godot;
using System;
using GamePasta.DungeonAlgorythms;


public class DungeonGeneration2D : TileMap
{

    [Export]
    private int _roomCount = 4;

    [Export]
    private int _segments = 6;
    [Export]
    private int _segmentSize = 15;

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
        GridDungeon dungeon = new GridDungeon(_segments, _segmentSize, _roomCount, new System.Numerics.Vector2(0, 2), new System.Numerics.Vector2(-2, 4));

        FeatureBuilder dungeonFeatures = new FeatureBuilder(dungeon);

        int[] tiles = new int[]{
            TileSet.FindTileByName("wall_1"),
            TileSet.FindTileByName("wall_2"),
            TileSet.FindTileByName("wall_3"),
        };

        int doorTile = TileSet.FindTileByName("door");
        int keyTile = TileSet.FindTileByName("key");
        int pressurePlateTile = TileSet.FindTileByName("pressure_plate");
        int treasureTile = TileSet.FindTileByName("chest");
        int secretDoorTile = TileSet.FindTileByName("secret_passage");

        int mapSize = _segments * (_segmentSize - 1);

        for (int i = 0; i < mapSize + 3; i++)
        {
            for (int j = 0; j < mapSize + 3; j++)
            {
                var tile = tiles[_random.Next(0, tiles.GetLength(0))];
                SetCell(i, j, tile);

            }
        }
        foreach (var n in dungeon.FullMask)
        {
            SetCell((int)n.X + 1, (int)n.Y + 1, -1);
        }
        foreach (var d in dungeonFeatures.Doors)
        {
            SetCell((int)d.Value.X + 1, (int)d.Value.Y + 1, doorTile);
        }
        foreach (var k in dungeonFeatures.DoorKeys)
        {
            SetCell((int)k.Value.X + 1, (int)k.Value.Y + 1, keyTile);
        }
        foreach (var g in dungeonFeatures.Gates)
        {
            SetCell((int)g.Value.X + 1, (int)g.Value.Y + 1, secretDoorTile);
        }
        foreach (var s in dungeonFeatures.GateSwitches)
        {
            SetCell((int)s.Value.X + 1, (int)s.Value.Y + 1, pressurePlateTile);
        }
        foreach (var t in dungeonFeatures.Treasures)
        {
            SetCell((int)t.Value.X + 1, (int)t.Value.Y + 1, treasureTile);
        }
        foreach (var tk in dungeonFeatures.TreasureKeys)
        {
            SetCell((int)tk.Value.X + 1, (int)tk.Value.Y + 1, keyTile);
        }
    }
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
        // mapSize = mapSize;
        Generate();
    }
}
