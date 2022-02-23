using System.Numerics;
using System.Collections.Generic;
using System;

namespace GamePasta.DungeonAlgorythms
{
    public class FeatureBuilder
    {
        private GridDungeon _dungeon;
        private Random _random = new Random();

        private Dictionary<Vector2, Vector2> _gates = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, Vector2> _gateSwitches = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, Vector2> _treasures = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, Vector2> _treasureKeys = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, Vector2> _doors = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, Vector2> _doorKeys = new Dictionary<Vector2, Vector2>();

        public Dictionary<Vector2, Vector2> Gates => _gates;

        public Dictionary<Vector2, Vector2> GateSwitches => _gateSwitches;

        public Dictionary<Vector2, Vector2> Treasures => _treasures;
        public Dictionary<Vector2, Vector2> TreasureKeys => _treasureKeys;
        public Dictionary<Vector2, Vector2> Doors => _doors;
        public Dictionary<Vector2, Vector2> DoorKeys => _doorKeys;

        private List<Vector2> _rooms = new List<Vector2>();
        public List<Vector2> Rooms => _rooms;

        public FeatureBuilder(GridDungeon dungeon)
        {
            _dungeon = dungeon;
            AddFeatures();
            AddRooms();
        }

        private void AddRooms()
        {
            List<Vector2> allSegments = new List<Vector2>();
            allSegments.AddRange(_dungeon.MainPath);
            foreach (var sp in _dungeon.SidePaths)
            {
                allSegments.AddRange(sp.Value);
            }

            foreach (var s in allSegments)
            {
                List<Rect> available = new List<Rect>(_dungeon.Chambers[s]);
                int clearings = _random.Next(1, 3);
                for (var i = 0; i <= clearings; i++)
                {
                    Rect clearing = available[_random.Next(0, available.Count - 1)];
                    Rect room = clearing.Expand(-1);
                    _rooms.AddRange(room.ToList());

                    available.Remove(clearing);
                }

            }
        }

        private void AddFeatures()
        {
            foreach (var k in _dungeon.MainPathBranches)
            {
                // check this path actually exists
                if (_dungeon.SidePaths.ContainsKey(k.Value[0]))
                {
                    int featureType = _random.Next(0, 100);

                    switch (featureType)
                    {
                        case int x when x <= 30 && x >= 0:
                            buildLockedDoorFeature(k);
                            break;
                        case int x when x > 30 && x <= 60:
                            buildSecretTreasureFeature(k);
                            break;
                        case int x when x > 60 && x <= 100:
                            buildLoopFeature(k);
                            break;
                    }
                }
            }
        }
        private void buildLoopFeature(KeyValuePair<Vector2, List<Vector2>> k)
        {
            List<Vector2> nodeList = _dungeon.SidePaths[k.Value[0]];
            // nodeList.Reverse();
            bool found = false;

            for (var i = nodeList.Count - 1; i > 0; i--)
            {

                var node = nodeList[i];
                var neighbours = Helpers.GetNeighbours(node);
                foreach (var n in neighbours)
                {

                    if (_dungeon.MainPath.Contains(n) && n != k.Key && n != _dungeon.MainPath[_dungeon.MainPath.Count - 1])
                    {
                        List<Vector2> connectionData = _dungeon.CreateConnections(n, node, _dungeon.MainDetail[n], _dungeon.SideDetail[node]);
                        ConnectionKey cKey = new ConnectionKey(n, node);
                        _dungeon.AddConnection(cKey.ToString(), connectionData);


                        var gate = PlaceDoor(connectionData, _dungeon.FullMask);
                        if (gate != null)
                        {
                            _gates[k.Key] = (Vector2)gate;
                            var gateKey = PlaceItem(_dungeon.Chambers[nodeList[0]]);
                            _gateSwitches[_gates[k.Key]] = gateKey;
                        }
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            if (!found) buildLockedDoorFeature(k);
        }

        private void buildSecretTreasureFeature(KeyValuePair<Vector2, List<Vector2>> k)
        {

            // Find corridor to side path and place a secret door / locked gate whatever
            ConnectionKey connKey = new ConnectionKey(k.Key, _dungeon.SidePaths[k.Value[0]][0]);
            var cor = _dungeon.ConnectionDetail[connKey.ToString()];
            var gate = PlaceDoor(cor, _dungeon.FullMask);

            if (gate != null) _gates[k.Key] = (Vector2)gate;
            else return;


            // find a place in current node to place a secret switch
            _gateSwitches[_gates[k.Key]] = PlaceItem(_dungeon.Chambers[k.Key]);


            // find a place in side path to place the treasure room
            Vector2 treasureMapNode = _dungeon.SidePaths[k.Value[0]][_dungeon.SidePaths[k.Value[0]].Count - 1];
            _treasures[k.Key] = PlaceItem(_dungeon.Chambers[treasureMapNode]);

            // find a place in side path to place the treasure key
            Vector2 keyMapNode = _dungeon.SidePaths[k.Value[0]][_random.Next(0, _dungeon.SidePaths[k.Value[0]].Count - 1)];
            _treasureKeys[_treasures[k.Key]] = PlaceItem(_dungeon.Chambers[keyMapNode]);
        }

        private void buildLockedDoorFeature(KeyValuePair<Vector2, List<Vector2>> k)
        {

            // get the main path exit from this node and place a locked door
            ConnectionKey connKey = new ConnectionKey(k.Key, _dungeon.MainPath[_dungeon.MainPath.FindIndex(item => item == k.Key) + 1]);
            var cor = _dungeon.ConnectionDetail[connKey.ToString()];
            var door = PlaceDoor(cor, _dungeon.FullMask);
            if (door != null) _doors[k.Key] = (Vector2)door;
            else return;

            // put the key in final room
            Vector2 keyMapNode = _dungeon.SidePaths[k.Value[0]][_dungeon.SidePaths[k.Value[0]].Count - 1];
            _doorKeys[_doors[k.Key]] = PlaceItem(_dungeon.Chambers[keyMapNode]);

        }
        private Vector2? PlaceDoor(List<Vector2> cor, List<Vector2> fullMask)
        {
            Vector2? door = null;
            foreach (var vec in cor)
            {
                byte bitMask = Helpers.getFourBitMask(_dungeon.FullMask, vec);

                if (bitMask == 9 || bitMask == 6)
                {
                    door = vec;
                    break;
                }
            };
            return door;
        }
        private Vector2 PlaceItem(List<Rect> rects)
        {
            Vector2 theKey;
            Rect chamber = rects[_random.Next(0, rects.Count - 1)];
            List<Vector2> chamberVecs = chamber.ToList();

            if (chamberVecs.Count == 1)
            {
                theKey = chamberVecs[0];
            }
            else
            {
                theKey = chamberVecs[_random.Next(0, chamberVecs.Count - 1)];
            }
            return theKey;
        }
    }
}

