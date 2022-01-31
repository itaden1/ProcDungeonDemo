using System.Collections.Generic;
using System;

namespace GamePasta.DungeonAlgorythms
{
    public class GridDungeon
    {
        private Random _random = new Random();
        private int _mapSize;
        private int _segments;
        private int _roomCount;


        private Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> _mainPath;
        public Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> MainPath => _mainPath;

        private Dictionary<System.Numerics.Vector2, System.Numerics.Vector2> _mainPathDoors =
            new Dictionary<System.Numerics.Vector2, System.Numerics.Vector2>();
        public Dictionary<System.Numerics.Vector2, System.Numerics.Vector2> MainPathDoors => _mainPathDoors;

        public GridDungeon(int mapSize, int segments, int roomCount)
        {
            _mapSize = mapSize;
            _segments = segments;
            _roomCount = roomCount;

        }
        public Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> Generate()
        {

            System.Numerics.Vector2 start = new System.Numerics.Vector2(0, 1);

            RandomWalk mainWalk = new RandomWalk(
                new System.Numerics.Vector2(_segments, _segments),
                start,
                new List<System.Numerics.Vector2>(),
                Direction.EAST,
                0
            );

            List<System.Numerics.Vector2> map = mainWalk.Execute();

            Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> segmentPaths = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>();

            segmentPaths = DigPath(map, new System.Numerics.Vector2(0, 0));
            _mainPath = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>(segmentPaths);


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
                RandomWalk sideWalk = new RandomWalk(
                    new System.Numerics.Vector2(5, 5),
                    neighbour,
                    mask,
                    Direction.NONE,
                    roomCount
                );
                List<System.Numerics.Vector2> sideWalkResult = sideWalk.Execute();
                secondaryPaths[vec] = sideWalkResult;
                mask.AddRange(sideWalkResult);
            }

            // connect the secondary paths
            foreach (var p in secondaryPaths)
            {
                Dictionary<string, System.Numerics.Vector2> entryExit = GetEntryExit(p.Value[0], p.Key);

                // correctly offset the exit before finding closest tile
                List<System.Numerics.Vector2> offsetExitVec = OffsetPath(p.Key, new List<System.Numerics.Vector2>() { entryExit["exit"] });
                System.Numerics.Vector2 exitVector = offsetExitVec[0];


                System.Numerics.Vector2 prevVector = Helpers.GetClosestVector(exitVector, segmentPaths[p.Key]);
                Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
                Rect nearvecRoom = new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3);


                List<Rect> corrs = Helpers.CreateCorridoor(
                    exitRoom,
                    nearvecRoom
                );
                foreach (Rect c in corrs)
                {
                    segmentPaths[p.Key].AddRange(c.ToList());
                }
                segmentPaths[p.Key].AddRange(exitRoom.ToList());
                segmentPaths[p.Key].AddRange(nearvecRoom.ToList());


                var newPath = DigPath(p.Value, entryExit["entry"]);
                foreach (var s in newPath)
                {
                    segmentPaths[s.Key] = s.Value;
                }
            }
            return segmentPaths;
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
            Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>> returnItems = new Dictionary<System.Numerics.Vector2, List<System.Numerics.Vector2>>();

            foreach (System.Numerics.Vector2 vec in map)
            {
                SimpleDig roomDigger = new SimpleDig(
                    new System.Numerics.Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                    start,
                    new System.Numerics.Vector2(5, 5),
                    6,
                    _roomCount
                );


                var tiles = roomDigger.Execute();

                // create an exit tile
                int nextMapTileIndex = map.FindIndex(item => item == vec) + 1;
                if (nextMapTileIndex < map.Count)
                {
                    System.Numerics.Vector2 nextMapTile = new System.Numerics.Vector2(map[nextMapTileIndex].X, map[nextMapTileIndex].Y);
                    Dictionary<string, System.Numerics.Vector2> entryExit = GetEntryExit(nextMapTile, vec);
                    start = entryExit["entry"];
                    System.Numerics.Vector2 exitVector = entryExit["exit"];

                    System.Numerics.Vector2 prevVector = Helpers.GetClosestVector(exitVector, tiles);
                    Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
                    tiles.AddRange(exitRoom.ToList());

                    List<System.Numerics.Vector2> corrTiles = new List<System.Numerics.Vector2>();

                    List<Rect> corrs = Helpers.CreateCorridoor(
                        exitRoom,
                        new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3)
                    );
                    foreach (Rect c in corrs)
                    {
                        corrTiles.AddRange(c.ToList());
                    }

                    // check the corridor for potential places to put a door
                    var door = FindPotentialDoorTile(corrTiles, tiles);
                    if (door.X > 0 && door.Y > 0)
                    {
                        var dList = new List<System.Numerics.Vector2>() { door };
                        _mainPathDoors[vec] = OffsetPath(vec, dList)[0];
                    }
                    tiles.AddRange(corrTiles);

                }
                returnItems[vec] = OffsetPath(vec, tiles);
            }
            return returnItems;
        }

        private System.Numerics.Vector2 FindPotentialDoorTile(List<System.Numerics.Vector2> corrTiles, List<System.Numerics.Vector2> mapTiles)
        {
            List<System.Numerics.Vector2> results = new List<System.Numerics.Vector2>();
            foreach (var t in corrTiles)
            {
                // wall tile on left and right
                var n = new System.Numerics.Vector2(t.X, t.Y - 1);
                var s = new System.Numerics.Vector2(t.X, t.Y + 1);
                var e = new System.Numerics.Vector2(t.X + 1, t.Y);
                var w = new System.Numerics.Vector2(t.X - 1, t.Y);

                if (!mapTiles.Contains(n) && !mapTiles.Contains(s))
                {
                    results.Add(t);
                }
                else if (!mapTiles.Contains(e) && !mapTiles.Contains(w))
                {
                    results.Add(t);
                }
            }
            if (results.Count == 0) return new System.Numerics.Vector2(-1, -1);
            if (results.Count > 2)
            {
                return results[_random.Next(1, results.Count - 1)];

            }
            else
            {
                return results[0];
            }
        }
    }
}

