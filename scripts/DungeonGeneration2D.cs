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
    private int _roomCount = 8;
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
        List<System.Numerics.Vector2> prevItems = new List<System.Numerics.Vector2>();
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

                    exitVector = new System.Numerics.Vector2(_mapSize / 4, yPos);
                    start = new System.Numerics.Vector2(0, yPos);
                }
                else if (nextMapTile.Y > vec.Y) // direction is south
                {
                    int xPos = _random.Next(2, (_mapSize / 4) - 2);

                    exitVector = new System.Numerics.Vector2(xPos, _mapSize / 4);
                    start = new System.Numerics.Vector2(xPos, 0);

                }
                else if (nextMapTile.Y < vec.Y) // north
                {
                    int xPos = _random.Next(2, (_mapSize / 4) - 2);

                    exitVector = new System.Numerics.Vector2(xPos, 0);
                    start = new System.Numerics.Vector2(xPos, _mapSize / 4);

                }
                else if (nextMapTile.X < vec.X) // west
                {
                    int yPos = _random.Next(2, (_mapSize / 4) - 2);
                    exitVector = new System.Numerics.Vector2(0, yPos);
                    start = new System.Numerics.Vector2(_mapSize / 4, yPos);
                }

                System.Numerics.Vector2 prevVector = GetClosestVector(exitVector, items);
                GD.Print($"{exitVector.X}:{exitVector.Y}");
                items.Add(exitVector);

                List<Rect> corrs = SimpleConnector.CreateCorridoor(
                    new Rect((int)exitVector.X, (int)exitVector.Y, 2, 2),
                    new Rect((int)prevVector.X, (int)prevVector.Y, 2, 2)
                );
                foreach (Rect c in corrs)
                {
                    items.AddRange(c.ToList());
                }

            }

            prevItems = items;

            foreach (System.Numerics.Vector2 p in items)
            {
                path.Add(new System.Numerics.Vector2(p.X + (_mapSize / 4 * vec.X), p.Y + (_mapSize / 4 * vec.Y)));
            }
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
