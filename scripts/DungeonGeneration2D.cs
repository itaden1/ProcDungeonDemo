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

        System.Numerics.Vector2 start = new System.Numerics.Vector2(0, 1);

        RandomWalk alg = new RandomWalk(
            new System.Numerics.Vector2(_segments, _segments),
            start,
            new List<System.Numerics.Vector2>(),
            Direction.EAST,
            0
        );

        List<System.Numerics.Vector2> map = alg.Execute();

        Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> segmentPaths = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>();

        segmentPaths = DigPath(map, new System.Numerics.Vector2(0, 0));

        Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> secondaryPaths = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>();

        // add the current map to our mask to not overwrite existing vectors
        List<System.Numerics.Vector2> mask = new List<System.Numerics.Vector2>();
        mask.AddRange(map);

        foreach (System.Numerics.Vector2 vec in map)
        {
            if (vec == map[map.Count - 1]) break; // we ar at the last room

            // check that this tile has none blocking neighbours
            List<System.Numerics.Vector2> neighbours = new List<System.Numerics.Vector2>()
            {
                // north
                new System.Numerics.Vector2(vec.X, vec.Y-1),
                // south
                new System.Numerics.Vector2(vec.X, vec.Y+1),
                //east
                new System.Numerics.Vector2(vec.X+1, vec.Y),
                //west
                new System.Numerics.Vector2(vec.X-1, vec.Y),

            };
            List<System.Numerics.Vector2> validNeighbours = new List<System.Numerics.Vector2>();


            foreach (var n in neighbours)
            {
                // TODO make map size configurable
                bool valid = true;
                if (n.X < 0 || n.X >= _segments - 1 || n.Y < 0 || n.Y >= _segments - 1 || mask.Contains(n)) valid = false;
                if (valid)
                {
                    validNeighbours.Add(n);
                }
            }
            if (validNeighbours.Count <= 0) continue;

            // choose a none blocking neighbour
            System.Numerics.Vector2 neighbour;
            if (validNeighbours.Count > 1)
            {
                neighbour = validNeighbours[_random.Next(0, validNeighbours.Count - 1)];
            }
            else
            {
                neighbour = validNeighbours[0];
            }

            // TODO if more than one neighbour choose a random amount of paths to create

            // TODO if neighbour is also neighbour to another main path tile roshambo

            int roomCount = _random.Next(1, 3);
            // execute random walk on the neighbour
            RandomWalk alg3 = new RandomWalk(
                new System.Numerics.Vector2(5, 5),
                neighbour,
                mask,
                Direction.NONE,
                roomCount
            );
            List<System.Numerics.Vector2> alg3Result = alg3.Execute();
            secondaryPaths[vec] = alg3Result;
            mask.AddRange(alg3Result);
        }

        // conect the secondary paths
        foreach (var p in secondaryPaths)
        {
            Dictionary<string, System.Numerics.Vector2> entryExit = GetEntryExit(p.Value[0], p.Key);

            // correctly offset the exit before finding closest tile
            List<System.Numerics.Vector2> offsetExitVec = OffsetPath(p.Key, new List<System.Numerics.Vector2>() { entryExit["exit"] });
            System.Numerics.Vector2 exitVector = offsetExitVec[0];


            System.Numerics.Vector2 prevVector = GetClosestVector(exitVector, segmentPaths[p.Key]);
            Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);

            List<Rect> corrs = SimpleConnector.CreateCorridoor(
                exitRoom,
                new Rect((int)prevVector.X, (int)prevVector.Y, 2, 2)
            );
            foreach (Rect c in corrs)
            {
                segmentPaths[p.Key].AddRange(c.ToList());
            }
            segmentPaths[p.Key].AddRange(exitRoom.ToList());


            var newPath = DigPath(p.Value, entryExit["entry"]);
            foreach (var s in newPath)
            {
                segmentPaths[s.Key] = s.Value;
            }
        }

        int[] tiles = new int[]{
            TileSet.FindTileByName("wall_1"),
            TileSet.FindTileByName("wall_2"),
            TileSet.FindTileByName("wall_3"),
        };

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
        }
    }

    private List<System.Numerics.Vector2> OffsetPath(System.Numerics.Vector2 vec, List<System.Numerics.Vector2> items)
    {
        // add an offset to each vector in the supplied list based of position of parent vector
        List<System.Numerics.Vector2> returnItems = new List<System.Numerics.Vector2>();
        foreach (System.Numerics.Vector2 p in items)
        {
            returnItems.Add(new System.Numerics.Vector2(p.X + (_mapSize / 4 * vec.X), p.Y + (_mapSize / 4 * vec.Y)));
        }
        return returnItems;
    }

    private Dictionary<string, System.Numerics.Vector2> GetEntryExit(System.Numerics.Vector2 to, System.Numerics.Vector2 from)
    {
        Dictionary<string, System.Numerics.Vector2> results = new Dictionary<string, System.Numerics.Vector2>();
        if (to.X > from.X) // direction is east
        {
            int yPos = _random.Next(2, (_mapSize / 4) - 2);

            results["exit"] = new System.Numerics.Vector2(_mapSize / 4 - 1, yPos);
            results["entry"] = new System.Numerics.Vector2(0, yPos);
        }
        else if (to.Y > from.Y) // direction is south
        {
            int xPos = _random.Next(2, (_mapSize / 4) - 2);

            results["exit"] = new System.Numerics.Vector2(xPos, _mapSize / 4 - 1);
            results["entry"] = new System.Numerics.Vector2(xPos, 0);

        }
        else if (to.Y < from.Y) // north
        {
            int xPos = _random.Next(2, (_mapSize / 4) - 2);

            results["exit"] = new System.Numerics.Vector2(xPos, 0);
            results["entry"] = new System.Numerics.Vector2(xPos, _mapSize / 4 - 1);

        }
        else if (to.X < from.X) // west
        {
            int yPos = _random.Next(2, (_mapSize / 4) - 2);

            results["exit"] = new System.Numerics.Vector2(0, yPos);
            results["entry"] = new System.Numerics.Vector2(_mapSize / 4 - 1, yPos);
        }
        return results;
    }

    private Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> DigPath(List<System.Numerics.Vector2> map, System.Numerics.Vector2 start)
    {
        // List<System.Numerics.Vector2> returnItems = new List<System.Numerics.Vector2>();

        Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> returnItems = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>();

        foreach (System.Numerics.Vector2 vec in map)
        {
            SimpleDig alg2 = new SimpleDig(
                new System.Numerics.Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                start,
                new System.Numerics.Vector2(5, 5),
                6,
                _roomCount
            );


            var items = alg2.Execute();

            // create an exit tile
            int nextMapTileIndex = map.FindIndex(item => item == vec) + 1;
            if (nextMapTileIndex < map.Count)
            {
                System.Numerics.Vector2 nextMapTile = new System.Numerics.Vector2(map[nextMapTileIndex].X, map[nextMapTileIndex].Y);
                System.Numerics.Vector2 exitVector = new System.Numerics.Vector2();
                Dictionary<string, System.Numerics.Vector2> entryExit = GetEntryExit(nextMapTile, vec);
                start = entryExit["entry"];
                exitVector = entryExit["exit"];

                System.Numerics.Vector2 prevVector = GetClosestVector(exitVector, items);
                Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
                items.AddRange(exitRoom.ToList());

                List<Rect> corrs = SimpleConnector.CreateCorridoor(
                    exitRoom,
                    new Rect((int)prevVector.X, (int)prevVector.Y, 3, 3)
                );
                foreach (Rect c in corrs)
                {
                    items.AddRange(c.ToList());
                }
            }
            returnItems[vec] = OffsetPath(vec, items);
            // returnItems.AddRange(OffsetPath(vec, items));
        }
        return returnItems;
    }

    private System.Numerics.Vector2 GetClosestVector(System.Numerics.Vector2 vec, List<System.Numerics.Vector2> vectors)
    {
        if (vectors.Count <= 0) return vec;
        System.Numerics.Vector2 closest = vectors[0];
        foreach (System.Numerics.Vector2 vector in vectors)
        {
            float newDistX = Math.Abs(vec.X - vector.X);
            float oldDistX = Math.Abs(vec.X - closest.X);
            float newDistY = Math.Abs(vec.X - vector.X);
            float oldDistY = Math.Abs(vec.X - closest.X);
            if (newDistX + newDistY < oldDistX + oldDistY)
            {
                closest = vector;
            }
        }
        return closest;
    }

    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
        _mapSize = mapSize;
        Generate();
    }
}
