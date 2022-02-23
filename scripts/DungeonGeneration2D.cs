using Godot;
using System;
using GamePasta.DungeonAlgorythms;
using System.Collections.Generic;

public class DungeonGeneration2D : TileMap
{

    [Export]
    private int _roomCount = 4;

    [Export]
    private int _segments = 5;
    [Export]
    private int _segmentSize = 16;

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
        GridDungeon dungeon = new GridDungeon(_segments, _segmentSize, _roomCount, new System.Numerics.Vector2(0, 2), new System.Numerics.Vector2(-2, 2));

        FeatureBuilder dungeonFeatures = new FeatureBuilder(dungeon);

        Dictionary<System.Numerics.Vector2, string> biomeMap = new Dictionary<System.Numerics.Vector2, string>();
        List<string> biomes = new List<string>() { "cave", "forrest", "catacombs" };
        for (var i = 0; i < dungeon.MainPath.Count; i++)
        {
            if (i < dungeon.MainPath.Count / 3)
            {
                biomeMap[dungeon.MainPath[i]] = "forrest";
            }
            else if (i < dungeon.MainPath.Count / 3 * 2)
            {
                biomeMap[dungeon.MainPath[i]] = "cave";
            }
            else
            {
                biomeMap[dungeon.MainPath[i]] = "catacombs";
            }
        }
        Dictionary<string, int> biomeTiles = new Dictionary<string, int>()
        {
            {"cave", TileSet.FindTileByName("cave")},
            {"forrest", TileSet.FindTileByName("tree")},
            {"catacombs", TileSet.FindTileByName("wall_3")}
        };

        // Dictionary<>

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
        int roomTile = TileSet.FindTileByName("room");


        var size = _segmentSize;

        foreach (var n in dungeon.MainPath)
        {
            var tile = biomeTiles[biomeMap[n]];
            for (int x = (int)n.X * size; x < ((int)n.X + 1) * size + 1; x++)
            {
                for (int y = (int)n.Y * size; y < ((int)n.Y + 1) * size + 1; y++)
                {
                    SetCell(x, y, tile);
                }
            }
            if (!dungeon.MainPathBranches.ContainsKey(n)) continue;


            var branch = dungeon.MainPathBranches[n];

            if (!dungeon.SidePaths.ContainsKey(branch[0])) continue;
            foreach (var sp in dungeon.SidePaths[branch[0]])
            {
                for (int x = (int)sp.X * size; x < ((int)sp.X + 1) * size + 1; x++)
                {
                    for (int y = (int)sp.Y * size; y < ((int)sp.Y + 1) * size + 1; y++)
                    {
                        SetCell(x, y, tile);
                    }
                }
            }
        }
        foreach (var n in dungeon.FullMask)
        {
            SetCell((int)n.X + 1, (int)n.Y + 1, -1);
        }
        foreach (var r in dungeonFeatures.Rooms)
        {
            SetCell((int)r.X + 1, (int)r.Y + 1, roomTile);
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
