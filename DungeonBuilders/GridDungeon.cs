using System.Collections.Generic;
using System;
using System.Numerics;

namespace GamePasta.DungeonAlgorythms
{
    public class GridDungeon
    {
        private Random _random = new Random();
        private int _mapSize;
        private int _segments;
        private int _roomCount;
        private List<Vector2> _mask = new List<Vector2>();

        private List<Vector2> _mainPath = new List<Vector2>();
        public List<Vector2> MainPath => _mainPath;
        private Dictionary<Vector2, List<Vector2>> _mainDetail = new Dictionary<Vector2, List<Vector2>>();
        public Dictionary<Vector2, List<Vector2>> MainDetail => _mainDetail;

        private Dictionary<Vector2, List<Vector2>> _sidePaths;
        public Dictionary<Vector2, List<Vector2>> SidePath => _sidePaths;

        private Dictionary<Vector2, List<Vector2>> _sideDetail = new Dictionary<Vector2, List<Vector2>>();
        public Dictionary<Vector2, List<Vector2>> SideDetail => _sideDetail;

        private Dictionary<Vector2, List<Vector2>> _mainPathBranches =
            new Dictionary<Vector2, List<Vector2>>();
        public Dictionary<Vector2, List<Vector2>> MainPathBranches => _mainPathBranches;

        private Dictionary<Vector2, Vector2> _mainPathDoors =
            new Dictionary<Vector2, Vector2>();
        public Dictionary<Vector2, Vector2> MainPathDoors => _mainPathDoors;

        private Dictionary<Vector2, Vector2> _mainPathKeys =
            new Dictionary<Vector2, Vector2>();
        public Dictionary<Vector2, Vector2> MainPathKeys => _mainPathKeys;

        public GridDungeon(int mapSize, int segments, int roomCount)
        {
            _mapSize = mapSize;
            _segments = segments;
            _roomCount = roomCount;
            Generate();
        }
        public void Generate()
        {

            Vector2 start = new Vector2(0, 1);

            RandomWalk mainWalk = new RandomWalk(
                new Vector2(_segments, _segments),
                start,
                new List<Vector2>(),
                Direction.EAST,
                0
            );

            _mainPath = mainWalk.Execute();

            // add the current map to our mask to not overwrite existing vectors
            _mask.AddRange(_mainPath);

            // create a maps for each node of the main path
            // segmentPaths = DigPath(map, new Vector2(0, 0), true);
            // _mainPath = new List<Vector2>(segmentPaths);

            // create secondary paths we use the main path node as a key so we can keep track of where
            _mainPathBranches = CreateBranches(_mainPath);
            // the path branches from
            // Dictionary<Vector2, List<Vector2>> secondaryPaths = CreateSecondaryPaths(_mainPathBranches);
            _sidePaths = CreateSecondaryPaths(_mainPathBranches);

            foreach (var n in _mainPath)
            {
                var p = DigPath(n, new Vector2(_random.Next(0, 5), _random.Next(0, 5)));
                _mainDetail[n] = OffsetPath(n, p);
                List<Vector2> corrTiles = new List<Vector2>();
            }
            foreach (var n in _mainPath)
            {
                var detail = _mainDetail[n];
                int nextMapTileIndex = _mainPath.FindIndex(item => item == n) + 1;
                if (nextMapTileIndex < _mainPath.Count)
                {
                    Vector2 nextMapTile = new Vector2(_mainPath[nextMapTileIndex].X, _mainPath[nextMapTileIndex].Y);
                    Dictionary<string, Vector2> entryExit = GetEntryExit(nextMapTile, n);
                    start = entryExit["entry"];
                    // Vector2 exitVector = entryExit["exit"];
                    Vector2 entryVector = OffsetPath(nextMapTile, new List<Vector2>() { entryExit["entry"] })[0];
                    Vector2 nextvector = Helpers.GetClosestVector(entryVector, _mainDetail[nextMapTile]);

                    Vector2 exitVector = OffsetPath(n, new List<Vector2>() { entryExit["exit"] })[0];

                    Vector2 prevVector = Helpers.GetClosestVector(exitVector, detail);
                    Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);

                    detail.AddRange(exitRoom.ToList());


                    List<Rect> corrs = Helpers.CreateCorridoor(
                        exitRoom,
                        // new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3),
                        new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3)
                    );
                    corrs.AddRange(Helpers.CreateCorridoor(
                        new Rect((int)entryVector.X, (int)entryVector.Y, 3, 3),
                        new Rect((int)nextvector.X, (int)nextvector.Y, 3, 3)
                    ));
                    foreach (Rect c in corrs)
                    {
                        // corrTiles.AddRange(c.ToList());
                        detail.AddRange(c.ToList());
                    }


                    // tiles.AddRange(corrTiles);

                }
            }


            foreach (var n in _sidePaths)
            {
                foreach (var np in n.Value)
                {
                    var p = DigPath(np, new Vector2(_random.Next(0, 5), _random.Next(0, 5)));
                    if (_sideDetail.ContainsKey(np))
                    {
                        _sideDetail[np].AddRange(OffsetPath(np, p));
                    }
                    else
                    {
                        _sideDetail[np] = OffsetPath(np, p);
                    }
                }
            }


            // connect the secondary paths
            // foreach (var p in secondaryPaths)
            // {
            //     Dictionary<string, Vector2> entryExit = GetEntryExit(p.Value[0], p.Key);

            //     // correctly offset the exit before finding closest tile
            //     List<Vector2> offsetExitVec = OffsetPath(p.Key, new List<Vector2>() { entryExit["exit"] });
            //     Vector2 exitVector = offsetExitVec[0];


            //     Vector2 prevVector = Helpers.GetClosestVector(exitVector, segmentPaths[p.Key]);
            //     Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
            //     Rect nearVecRoom = new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3);


            //     List<Rect> corrs = Helpers.CreateCorridoor(
            //         exitRoom,
            //         nearVecRoom
            //     );
            //     foreach (Rect c in corrs)
            //     {
            //         segmentPaths[p.Key].AddRange(c.ToList());
            //     }
            //     segmentPaths[p.Key].AddRange(exitRoom.ToList());
            //     segmentPaths[p.Key].AddRange(nearVecRoom.ToList());




            //     var newPath = DigPath(p.Value, entryExit["entry"]);
            //     foreach (var s in newPath)
            //     {
            //         bool keyPlaced = false;
            //         segmentPaths[s.Key] = s.Value;
            //         // key required to unlock a door
            //         if (_mainPathDoors.ContainsKey(p.Key) && !keyPlaced)
            //         {
            //             Vector2 location = s.Value[_random.Next(1, s.Value.Count) - 1];
            //             _mainPathKeys[p.Key] = location;
            //             keyPlaced = true;
            //             Godot.GD.Print("Place a key or sumfin");
            //         }

            //     }

            // }
            // return segmentPaths;
        }

        private Dictionary<Vector2, List<Vector2>> CreateBranches(List<Vector2> path)
        {
            Dictionary<Vector2, List<Vector2>> branches = new Dictionary<Vector2, List<Vector2>>();
            List<Vector2> neighbourMask = new List<Vector2>();
            neighbourMask.AddRange(_mask);
            foreach (Vector2 vec in path)
            {
                if (vec == path[path.Count - 1]) break; // we ar at the last room
                // check that this tile has none blocking neighbours
                List<Vector2> neighbours = new List<Vector2>()
                {
                    // north
                    new Vector2(vec.X, vec.Y-1),
                    // south
                    new Vector2(vec.X, vec.Y+1),
                    //east
                    new Vector2(vec.X+1, vec.Y),
                    //west
                    new Vector2(vec.X-1, vec.Y),

                };
                List<Vector2> validNeighbours = new List<Vector2>();


                foreach (var n in neighbours)
                {
                    // TODO make map size configurable
                    bool valid = true;
                    if (n.X < 0 || n.X >= _segments - 1 || n.Y < 0 || n.Y >= _segments - 1 || neighbourMask.Contains(n)) valid = false;
                    if (valid)
                    {
                        validNeighbours.Add(n);
                    }
                }
                if (validNeighbours.Count <= 0)
                {
                    continue;
                }

                // choose a none blocking neighbour
                Vector2 neighbour;
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
                if (branches.ContainsKey(vec))
                {
                    branches[vec].Add(neighbour);
                }
                else
                {
                    branches[vec] = new List<Vector2>() { neighbour };

                }
                neighbourMask.Add(neighbour);
            }
            Godot.GD.Print("*****");
            return branches;
        }

        private Dictionary<Vector2, List<Vector2>> CreateSecondaryPaths(Dictionary<Vector2, List<Vector2>> branchPoints)
        {
            Dictionary<Vector2, List<Vector2>> paths = new Dictionary<Vector2, List<Vector2>>();
            foreach (var branch in branchPoints)
            {
                foreach (var vec in branch.Value)
                {
                    Godot.GD.Print($"({branch.Key.X},{branch.Key.Y}) -> ({vec.X},{vec.Y})");
                    int roomCount = _random.Next(2, 6);

                    // execute random walk on the neighbour
                    RandomWalk sideWalk = new RandomWalk(
                        new Vector2(5, 5),
                        vec,
                        _mask,
                        Direction.NONE,
                        roomCount
                    );

                    List<Vector2> sideWalkResult = sideWalk.Execute();

                    // var k = sideWalkResult[0];
                    // sideWalkResult.RemoveAt(0);
                    if (paths.ContainsKey(vec))
                    {
                        paths[vec].AddRange(sideWalkResult);
                    }
                    else
                    {
                        paths[vec] = sideWalkResult;
                    }
                    _mask.AddRange(sideWalkResult);
                }

            }
            return paths;
        }

        private List<Vector2> OffsetPath(Vector2 vec, List<Vector2> items)
        {
            // add an offset to each vector in the supplied list based of position of parent vector
            List<Vector2> returnItems = new List<Vector2>();
            foreach (Vector2 p in items)
            {
                returnItems.Add(new Vector2(p.X + (_mapSize / 4 * vec.X), p.Y + (_mapSize / 4 * vec.Y)));
            }
            return returnItems;
        }

        private Dictionary<string, Vector2> GetEntryExit(Vector2 to, Vector2 from)
        {
            Dictionary<string, Vector2> results = new Dictionary<string, Vector2>();
            if (to.X > from.X) // direction is east
            {
                int yPos = _random.Next(2, (_mapSize / 4) - 2);

                results["exit"] = new Vector2(_mapSize / 4 - 1, yPos);
                results["entry"] = new Vector2(0, yPos);
            }
            else if (to.Y > from.Y) // direction is south
            {
                int xPos = _random.Next(2, (_mapSize / 4) - 2);

                results["exit"] = new Vector2(xPos, _mapSize / 4 - 1);
                results["entry"] = new Vector2(xPos, 0);

            }
            else if (to.Y < from.Y) // north
            {
                int xPos = _random.Next(2, (_mapSize / 4) - 2);

                results["exit"] = new Vector2(xPos, 0);
                results["entry"] = new Vector2(xPos, _mapSize / 4 - 1);

            }
            else if (to.X < from.X) // west
            {
                int yPos = _random.Next(2, (_mapSize / 4) - 2);

                results["exit"] = new Vector2(0, yPos);
                results["entry"] = new Vector2(_mapSize / 4 - 1, yPos);
            }
            return results;
        }

        private List<Vector2> DigPath(
            Vector2 node,
            Vector2 start,
            bool placeDoors = false)
        {
            // List<Vector2> returnItems = new List<Vector2>();

            // foreach (Vector2 vec in map)
            // {
            SimpleDig roomDigger = new SimpleDig(
                new Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                start,
                new Vector2(5, 5),
                6,
                _roomCount
            );
            return roomDigger.Execute();
            // returnItems.AddRange(roomDigger.Execute());
            //     List<Vector2> corrTiles = new List<Vector2>();

            //     var tiles = roomDigger.Execute();
            //     int nextMapTileIndex = map.FindIndex(item => item == vec) + 1;
            //     if (nextMapTileIndex < map.Count)
            //     {
            //         Vector2 nextMapTile = new Vector2(map[nextMapTileIndex].X, map[nextMapTileIndex].Y);
            //         Dictionary<string, Vector2> entryExit = GetEntryExit(nextMapTile, vec);
            //         start = entryExit["entry"];
            //         Vector2 exitVector = entryExit["exit"];

            //         Vector2 prevVector = Helpers.GetClosestVector(exitVector, tiles);
            //         Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);
            //         tiles.AddRange(exitRoom.ToList());


            //         List<Rect> corrs = Helpers.CreateCorridoor(
            //             exitRoom,
            //             new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3)
            //         );
            //         foreach (Rect c in corrs)
            //         {
            //             corrTiles.AddRange(c.ToList());
            //         }


            //         tiles.AddRange(corrTiles);

            //     }
            //     if (placeDoors)
            //     {
            //         // check the corridors for potential places to put a door
            //         var door = FindPotentialDoorTile(corrTiles, tiles);
            //         if (door.X > 0 && door.Y > 0)
            //         {
            //             var dList = new List<Vector2>() { door };
            //             _mainPathDoors[vec] = OffsetPath(vec, dList)[0];
            //         }
            //     }

            //     returnItems[vec] = OffsetPath(vec, tiles);
            // }
            // return returnItems;

        }

    }
}

