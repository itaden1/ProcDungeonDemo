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
            new System.Numerics.Vector2(5, 5),
            start,
            new List<System.Numerics.Vector2>(),
            Direction.EAST,
            0
        );

        List<System.Numerics.Vector2> map = alg.Execute();

        List<System.Numerics.Vector2> path = new List<System.Numerics.Vector2>();

        path.AddRange(DigPath(map));


        //loop through the map again and create secondary passages
        List<List<System.Numerics.Vector2>> secondaryPaths = new List<List<System.Numerics.Vector2>>();

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
                bool valid = true;
                foreach (var m in mask)
                {
                    if (n.X == m.X && n.Y == m.Y)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    validNeighbours.Add(n);
                }
            }
            if (validNeighbours.Count <= 0) continue;
            GD.Print(validNeighbours.Count);

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

            // execute random walk on the neighbour
            RandomWalk alg3 = new RandomWalk(
                new System.Numerics.Vector2(5, 5),
                neighbour,
                mask,
                Direction.NONE,
                3
            );
            List<System.Numerics.Vector2> alg3Result = alg3.Execute();
            secondaryPaths.Add(alg3Result);
            mask.AddRange(alg3Result);
        }

        foreach (var p in secondaryPaths)
        {
            path.AddRange(DigPath(p));
        }
        // List<System.Numerics.Vector2> path = alg2.Execute();
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
        foreach (System.Numerics.Vector2 vec in path)
        {
            // var tile = tiles[_random.Next(0, tiles.GetLength(0))];
            SetCell((int)vec.X + 1, (int)vec.Y + 1, -1);
        }


    }

    private IEnumerable<System.Numerics.Vector2> DigPath(List<System.Numerics.Vector2> map)
    {
        List<System.Numerics.Vector2> prevItems = new List<System.Numerics.Vector2>();
        List<System.Numerics.Vector2> returnItems = new List<System.Numerics.Vector2>();

        System.Numerics.Vector2 start = new System.Numerics.Vector2(0, 0);
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

                if (nextMapTile.X > vec.X) // direction is east
                {
                    int yPos = _random.Next(2, (_mapSize / 4) - 2);

                    exitVector = new System.Numerics.Vector2(_mapSize / 4 - 2, yPos);
                    start = new System.Numerics.Vector2(0, yPos);
                }
                else if (nextMapTile.Y > vec.Y) // direction is south
                {
                    int xPos = _random.Next(2, (_mapSize / 4) - 2);

                    exitVector = new System.Numerics.Vector2(xPos, _mapSize / 4 - 2);
                    start = new System.Numerics.Vector2(xPos, 0);

                }
                else if (nextMapTile.Y < vec.Y) // north
                {
                    int xPos = _random.Next(2, (_mapSize / 4) - 2);

                    exitVector = new System.Numerics.Vector2(xPos, 0);
                    start = new System.Numerics.Vector2(xPos, _mapSize / 4 - 2);

                }
                else if (nextMapTile.X < vec.X) // west
                {
                    int yPos = _random.Next(2, (_mapSize / 4) - 2);
                    exitVector = new System.Numerics.Vector2(0, yPos);
                    start = new System.Numerics.Vector2(_mapSize / 4 - 2, yPos);
                }

                System.Numerics.Vector2 prevVector = GetClosestVector(exitVector, items);
                Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
                items.AddRange(exitRoom.ToList());

                List<Rect> corrs = SimpleConnector.CreateCorridoor(
                    exitRoom,
                    new Rect((int)prevVector.X, (int)prevVector.Y, 1, 1)
                );
                foreach (Rect c in corrs)
                {
                    items.AddRange(c.ToList());
                }

            }

            prevItems = items;

            foreach (System.Numerics.Vector2 p in items)
            {
                returnItems.Add(new System.Numerics.Vector2(p.X + (_mapSize / 4 * vec.X), p.Y + (_mapSize / 4 * vec.Y)));
            }
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
