using System.Collections.Generic;
using System;
using System.Numerics;

namespace GamePasta.DungeonAlgorythms
{
    public class ConnectionKey
    {
        private Vector2 _source;
        public Vector2 Source => _source;
        private Vector2 _destination;
        public Vector2 Destination => _destination;
        public ConnectionKey(Vector2 from, Vector2 to)
        {
            _source = from;
            _destination = to;
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            return Equals(obj);
        }
        public bool Equals(ConnectionKey other)
        {
            return (other.Source == Source && other.Destination == Destination);
        }
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

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


        private Dictionary<ConnectionKey, List<Vector2>> _connectionDetail =
            new Dictionary<ConnectionKey, List<Vector2>>();
        public Dictionary<ConnectionKey, List<Vector2>> ConnectionDetail => _connectionDetail;

        private Dictionary<Vector2, List<Rect>> _chambers =
            new Dictionary<Vector2, List<Rect>>();
        public Dictionary<Vector2, List<Rect>> Chambers => _chambers;

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


            // create secondary paths we use the main path node as a key so we can keep track of where
            // the path branches from

            _mainPathBranches = CreateBranches(_mainPath);
            _sidePaths = CreateSecondaryPaths(_mainPathBranches);

            // dig out the level details for each node on the main path
            foreach (var n in _mainPath)
            {
                SimpleDig roomDigger = new SimpleDig(
                       new Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                       new Vector2(_random.Next(0, 5), _random.Next(0, 5)),
                       new Vector2(5, 5),
                       6,
                       _roomCount
                   );
                var p = roomDigger.Execute();
                _mainDetail[n] = OffsetPath(n, p);
                List<Vector2> corrTiles = new List<Vector2>();
            }

            //connect each node in main path
            foreach (var n in _mainPath)
            {
                var detail = _mainDetail[n];
                int nextMapTileIndex = _mainPath.FindIndex(item => item == n) + 1;
                if (nextMapTileIndex < _mainPath.Count)
                {
                    Vector2 nextMapTile = new Vector2(_mainPath[nextMapTileIndex].X, _mainPath[nextMapTileIndex].Y);
                    Dictionary<string, Vector2> entryExit = GetEntryExit(nextMapTile, n);
                    List<Vector2> connectionData = CreateConnections(n, nextMapTile, _mainDetail[n], _mainDetail[nextMapTile]);
                    ConnectionKey conKey = new ConnectionKey(n, nextMapTile);
                    _connectionDetail[conKey] = connectionData;

                }
            }

            // dig out the level details for each side path node
            foreach (var n in _sidePaths)
            {
                foreach (var np in n.Value)
                {
                    SimpleDig roomDigger = new SimpleDig(
                        new Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                        new Vector2(_random.Next(0, 5), _random.Next(0, 5)),
                        new Vector2(5, 5),
                        6,
                        _roomCount
                    );
                    var p = roomDigger.Execute();
                    if (_sideDetail.ContainsKey(np))
                    {
                        _sideDetail[np].AddRange(OffsetPath(np, p));
                    }
                    else
                    {
                        _sideDetail[np] = OffsetPath(np, p);
                    }

                    // offset the generated rooms and add them to rooms dict
                    var rooms = roomDigger.GetRooms();
                    List<Rect> offsetRooms = new List<Rect>();
                    foreach (Rect r in rooms)
                    {
                        Vector2 newPos = OffsetPath(np, new List<Vector2>() { r.Start })[0];
                        offsetRooms.Add(r.Move((int)newPos.X, (int)newPos.Y));
                    }
                    _chambers[np] = offsetRooms;
                }
            }

            foreach (var n in _sidePaths)
            {
                // connect each node on side path
                foreach (var np in n.Value)
                {
                    var detail = _sideDetail[np];
                    int nextMapTileIndex = n.Value.FindIndex(item => item == np) + 1;
                    if (nextMapTileIndex < n.Value.Count)
                    {
                        Vector2 nextMapTile = new Vector2(n.Value[nextMapTileIndex].X, n.Value[nextMapTileIndex].Y);
                        Dictionary<string, Vector2> entryExit = GetEntryExit(nextMapTile, np);
                        List<Vector2> connectionData = CreateConnections(np, nextMapTile, _sideDetail[np], _sideDetail[nextMapTile]);
                        ConnectionKey conKey = new ConnectionKey(np, nextMapTile);
                        _connectionDetail[conKey] = connectionData;

                    }
                }
                // find the connection point between the main path and this side path
                Vector2 mainBranchKey = new Vector2();
                foreach (var b in _mainPathBranches)
                {
                    if (b.Value[0] == n.Key) mainBranchKey = b.Key;
                }
                List<Vector2> connData = CreateConnections(mainBranchKey, n.Key, _mainDetail[mainBranchKey], _sideDetail[n.Key]);
                ConnectionKey cKey = new ConnectionKey(n.Key, n.Value[0]);
                _connectionDetail[cKey] = connData;

            }

            // Find places to place locked doors
            foreach (var k in _mainPathBranches)
            {
                // final room of the side path
                if (_sidePaths.ContainsKey(k.Value[0]))
                {
                    // get the main path exit
                    ConnectionKey connKey = new ConnectionKey(k.Key, _mainPath[_mainPath.FindIndex(item => item == k.Key) + 1]);
                    // var cor = _connectionDetail[connKey];
                    // Godot.GD.Print(cor);

                    Vector2 keyMapNode = _sidePaths[k.Value[0]][_sidePaths[k.Value[0]].Count - 1];
                    foreach (var c in _chambers[keyMapNode])
                    {
                        // _mainPathKeys[] = ;
                    }
                }
            }
            // find place to put the key
        }

        private List<Vector2> CreateConnections(Vector2 tile, Vector2 nextTile, List<Vector2> detail, List<Vector2> nextDetail)
        {
            List<Vector2> returnData = new List<Vector2>();
            Dictionary<string, Vector2> entryExit = GetEntryExit(nextTile, tile);
            // Vector2 exitVector = entryExit["exit"];
            Vector2 entryVector = OffsetPath(nextTile, new List<Vector2>() { entryExit["entry"] })[0];
            Vector2 nextvector = Helpers.GetClosestVector(entryVector, nextDetail);

            Vector2 exitVector = OffsetPath(tile, new List<Vector2>() { entryExit["exit"] })[0];

            Vector2 prevVector = Helpers.GetClosestVector(exitVector, detail);
            Rect exitRoom = new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3);

            Rect entryRoom = new Rect((int)entryVector.X, (int)entryVector.Y, 3, 3);

            returnData.AddRange(exitRoom.ToList());
            returnData.AddRange(entryRoom.ToList());



            List<Rect> corrs = Helpers.CreateCorridoor(
                exitRoom,
                // new Rect((int)exitVector.X, (int)exitVector.Y, 3, 3),
                new Rect((int)prevVector.X - 1, (int)prevVector.Y - 1, 3, 3)
            );
            corrs.AddRange(Helpers.CreateCorridoor(
                entryRoom,
                new Rect((int)nextvector.X, (int)nextvector.Y, 3, 3)
            ));
            foreach (Rect c in corrs)
            {
                // corrTiles.AddRange(c.ToList());
                returnData.AddRange(c.ToList());
            }

            return returnData;
            // tiles.AddRange(corrTiles);
        }

        private Dictionary<Vector2, List<Vector2>> CreateBranches(List<Vector2> path)
        {
            Dictionary<Vector2, List<Vector2>> branches = new Dictionary<Vector2, List<Vector2>>();
            List<Vector2> neighbourMask = new List<Vector2>();
            neighbourMask.AddRange(path);
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
                    Godot.GD.Print(neighbourMask.Contains(n));
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
            Godot.GD.Print(branches.Count);

            Godot.GD.Print("*****");
            return branches;
        }

        private Dictionary<Vector2, List<Vector2>> CreateSecondaryPaths(Dictionary<Vector2, List<Vector2>> branchPoints)
        {
            Dictionary<Vector2, List<Vector2>> paths = new Dictionary<Vector2, List<Vector2>>();
            List<Vector2> mask = new List<Vector2>();
            foreach (var branch in branchPoints)
            {
                foreach (var vec in branch.Value)
                {
                    if (_mask.Contains(vec)) continue;  // we may want to roll to see which path gets created

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
            SimpleDig roomDigger = new SimpleDig(
                new Vector2(_mapSize / 4 - 2, _mapSize / 4 - 2),
                start,
                new Vector2(5, 5),
                6,
                _roomCount
            );
            return roomDigger.Execute();
        }

    }
}

