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

        var gyDone = false;
        for (var i = 0; i < dungeon.MainPath.Count; i++)
        {
            if (i < dungeon.MainPath.Count / 3)
            {
                biomeMap[dungeon.MainPath[i]] = "forrest";
                if (dungeon.MainPathBranches.ContainsKey(dungeon.MainPath[i]))
                {
                    var branch = dungeon.MainPathBranches[dungeon.MainPath[i]];

                    if (!dungeon.SidePaths.ContainsKey(branch[0])) continue;

                    foreach (var sp in dungeon.SidePaths[branch[0]])
                    {
                        if (gyDone)
                        {
                            biomeMap[sp] = "forrest";
                        }
                        else
                        {
                            biomeMap[sp] = "graveyard";
                        }
                    }
                    gyDone = true;

                }
            }
            else if (i < dungeon.MainPath.Count / 3 * 2)
            {
                biomeMap[dungeon.MainPath[i]] = "cave";
                if (dungeon.MainPathBranches.ContainsKey(dungeon.MainPath[i]))
                {
                    var branch = dungeon.MainPathBranches[dungeon.MainPath[i]];

                    if (!dungeon.SidePaths.ContainsKey(branch[0])) continue;
                    foreach (var sp in dungeon.SidePaths[branch[0]])
                    {
                        biomeMap[sp] = "cave";
                    }
                }
            }
            else
            {
                biomeMap[dungeon.MainPath[i]] = "catacombs";
                if (dungeon.MainPathBranches.ContainsKey(dungeon.MainPath[i]))
                {
                    var branch = dungeon.MainPathBranches[dungeon.MainPath[i]];

                    if (!dungeon.SidePaths.ContainsKey(branch[0])) continue;
                    foreach (var sp in dungeon.SidePaths[branch[0]])
                    {
                        biomeMap[sp] = "catacombs";
                    }
                }
            }
        }
        Dictionary<string, int> biomeTiles = new Dictionary<string, int>()
        {
            {"cave", TileSet.FindTileByName("cave")},
            {"forrest", TileSet.FindTileByName("tree")},
            {"graveyard", TileSet.FindTileByName("tree2")},
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
        int pitTile = TileSet.FindTileByName("pit");


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
        }
        foreach (var sp in dungeon.SidePaths)
        {
            foreach (var n in sp.Value)
            {
                var tile = biomeTiles[biomeMap[n]];
                for (int x = (int)n.X * size; x < ((int)n.X + 1) * size + 1; x++)
                {
                    for (int y = (int)n.Y * size; y < ((int)n.Y + 1) * size + 1; y++)
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
        foreach (var p in dungeonFeatures.ExtraLoops)
        {
            if (biomeMap[p.Key] == "forrest")
            {
                foreach (var v in p.Value)
                {
                    SetCell((int)v.X, (int)v.Y, -1);
                }
            }
        }
        foreach (var r in dungeonFeatures.Rooms)
        {
            SetCell((int)r.X + 1, (int)r.Y + 1, roomTile);
        }
        foreach (var d in dungeonFeatures.Doors)
        {
            SetCell((int)d.Value.X + 1, (int)d.Value.Y + 1, doorTile);
            var doorKey = dungeonFeatures.DoorKeys[d.Value];
            SetCell((int)doorKey.X + 1, (int)doorKey.Y + 1, keyTile);

        }

        foreach (var g in dungeonFeatures.Gates)
        {
            SetCell((int)g.Value.X + 1, (int)g.Value.Y + 1, secretDoorTile);
            var gateSwitch = dungeonFeatures.GateSwitches[g.Value];
            SetCell((int)gateSwitch.X + 1, (int)gateSwitch.Y + 1, pressurePlateTile);
        }
        foreach (var t in dungeonFeatures.Treasures)
        {
            SetCell((int)t.Value.X + 1, (int)t.Value.Y + 1, treasureTile);
            var treasureKey = dungeonFeatures.TreasureKeys[t.Value];
            SetCell((int)treasureKey.X + 1, (int)treasureKey.Y + 1, keyTile);
        }

    }
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
        // mapSize = mapSize;
        Generate();
    }
}
