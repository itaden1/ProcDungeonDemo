using System;
using System.Numerics;
using System.Collections.Generic;

namespace GamePasta.DungeonAlgorythms
{
    public struct Rect
    {
        public Vector2 Start;
        public Vector2 Size;
        public Vector2 End;
        public Vector2 Mid;
        public Rect(Vector2 position, Vector2 size)
        {
            Start = position;
            Size = size;
            End = new Vector2(position.X + size.X, position.Y + size.Y);
            Mid = new Vector2((int)(End.X - Size.X / 2), (int)(End.Y - Size.Y / 2));
        }
        public Rect(int x, int y, int width, int height)
        {
            Start = new Vector2(x, y);
            Size = new Vector2(width, height);
            End = new Vector2(x + width, y + height);
            Mid = new Vector2((int)(End.X - Size.X / 2), (int)(End.Y - Size.Y / 2));
        }
        public Rect Expand(int amount)
        {
            // returns a rect representing the margin
            return new Rect(
                new Vector2(Start.X - amount, Start.Y - amount),
                new Vector2(Size.X + amount * 2, Size.Y + amount * 2)
            );
        }
        public bool Intersects(Rect other)
        {
            return (
                other.Start.X < End.X && other.End.X > Start.X
                && other.Start.Y < End.Y && other.End.Y > Start.Y
            );
        }
        public Rect Move(int x, int y)
        {
            return new Rect(x, y, (int)Size.X, (int)Size.Y);

        }
        public List<Vector2> ToList()
        {
            List<Vector2> result = new List<Vector2>();
            for (int x = (int)Start.X; x < End.X; x++)
            {
                for (int y = (int)Start.Y; y < End.Y; y++)
                {
                    result.Add(new Vector2(x, y));
                }
            }
            return result;
        }
    }
    public class SimpleDig : IRandomAlgorythm
    {
        private Vector2 _gridSize;
        private Vector2 _start;
        private int _maxSteps;
        private Direction _direction;
        private Vector2 _maxRoomSize;
        private Vector2 _minRoomSize = new Vector2(2, 2);

        private int _maxChambers;
        private List<Rect> _rooms = new List<Rect>();
        private Dictionary<Direction, Vector2> _directionMap;
        public SimpleDig(Vector2 gridSize, Vector2 start, Vector2 maxRoomSize, int maxChambers, int maxSteps)
        {
            _gridSize = gridSize;
            _start = start;
            _maxRoomSize = maxRoomSize;
            _maxSteps = maxSteps;
            _maxChambers = maxChambers;
        }

        public List<Vector2> Execute()
        {
            Random rand = new Random();
            List<Vector2> path = new List<Vector2>();

            List<Rect> rects = new List<Rect>();
            List<Rect> corridoors = new List<Rect>();

            // rects.Add(new Rect((int)_start.X, (int)_start.Y, 3, 3));

            int threshold = 1000;
            while (rects.Count < _maxSteps)
            {
                // create a rect
                int width = rand.Next((int)_minRoomSize.X, (int)_maxRoomSize.X);
                int height = rand.Next((int)_minRoomSize.Y, (int)_maxRoomSize.Y);
                int positionX = rand.Next(0, (int)_gridSize.X - width);
                int positionY = rand.Next(0, (int)_gridSize.Y - height);

                bool validPlacement = true;
                Rect rect = new Rect(new Vector2(positionX, positionY), new Vector2(width, height));
                Rect bounds = rect.Expand(1);
                foreach (Rect r in rects)
                {
                    if (bounds.Intersects(r))
                    {
                        validPlacement = false;
                    }
                }
                if (validPlacement)
                {
                    _rooms.Add(rect);
                    path.AddRange(rect.ToList());
                    if (rects.Count >= 1)
                    {
                        var corridoor = Helpers.CreateCorridoor(rect, rects[rects.Count - 1]);
                        corridoors.AddRange(corridoor);
                        foreach (Rect c in corridoor)
                        {
                            path.AddRange(c.ToList());
                        }
                    }
                    rects.Add(rect);
                }
                threshold--;
                if (threshold <= 0) break;
            }

            // fill empty space with chambers
            List<Rect> potentialChambers = new List<Rect>();
            List<Rect> blockages = new List<Rect>();
            blockages.AddRange(corridoors);
            blockages.AddRange(rects);

            Vector2 size = new Vector2(_maxRoomSize.X, _maxRoomSize.Y);

            while (potentialChambers.Count <= _maxChambers)
            {
                for (int x = 1; x < _gridSize.X; x++)
                {
                    bool placeable = true;

                    for (int y = 1; y < _gridSize.Y; y++)
                    {

                        Rect potentialChamber = new Rect(x, y, (int)size.X, (int)size.Y);
                        Rect margin = potentialChamber.Expand(1);
                        placeable = true;

                        foreach (Rect b in blockages)
                        {

                            if (margin.Intersects(b) || margin.End.X > _gridSize.X || margin.End.Y > _gridSize.Y)
                            {
                                placeable = false;
                                break;
                            }
                        }
                        if (placeable)
                        {
                            blockages.Add(potentialChamber);
                            potentialChambers.Add(potentialChamber);
                            break;
                        }
                    }
                }
                size.X--;
                size.Y--;
                if (size.X < 1 || size.Y < 1) break;
            }

            List<Vector2> validPathNodes = new List<Vector2>();
            List<Rect> validChambers = new List<Rect>();


            foreach (Vector2 p in path)
            {
                List<Vector2> directions = new List<Vector2>();
                directions.Add(new Vector2(p.X, p.Y - 2)); // north
                directions.Add(new Vector2(p.X, p.Y + 2)); // south
                directions.Add(new Vector2(p.X - 2, p.Y)); // west
                directions.Add(new Vector2(p.X + 2, p.Y)); // east

                foreach (Vector2 d in directions)
                {
                    foreach (Rect c in potentialChambers)
                    {

                        if (c.Intersects(new Rect(d, new Vector2(1, 1))))
                        {
                            validPathNodes.Add(p);
                            validChambers.Add(c);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < _maxChambers; i++)
            {
                Vector2 nodeChoice;
                if (validPathNodes.Count > 1)
                {
                    nodeChoice = validPathNodes[rand.Next(0, validPathNodes.Count - 1)];
                }
                else if (validPathNodes.Count == 1)
                {
                    nodeChoice = validPathNodes[0];
                }
                else break;

                List<Vector2> directions = new List<Vector2>();
                directions.Add(new Vector2(nodeChoice.X, nodeChoice.Y - 1)); // north
                directions.Add(new Vector2(nodeChoice.X, nodeChoice.Y + 1)); // south
                directions.Add(new Vector2(nodeChoice.X - 1, nodeChoice.Y)); // west
                directions.Add(new Vector2(nodeChoice.X + 1, nodeChoice.Y)); // east


                Vector2 dChoice = directions[rand.Next(0, directions.Count - 1)];

                foreach (Rect c in validChambers)
                {
                    if (c.Intersects(new Rect(dChoice, new Vector2(3, 3))))
                    {
                        List<Rect> connectors = Helpers.CreateCorridoor(c, new Rect(dChoice, new Vector2(1, 1)));
                        _rooms.Add(c);
                        path.AddRange(c.ToList());
                        foreach (Rect conn in connectors)
                        {
                            path.AddRange(conn.ToList());
                        }
                        rects.Add(c);
                        break;
                    }
                }
                validPathNodes.Remove(nodeChoice);
            }

            return path;
        }
        public List<Rect> GetRooms()
        {
            return _rooms;
        }

    }
}

