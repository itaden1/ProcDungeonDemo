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
        private List<Rect> _rooms = new List<Rect>();
        private Dictionary<Direction, Vector2> _directionMap;
        public SimpleDig(Vector2 gridSize, Vector2 start, Vector2 maxRoomSize, int maxSteps)
        {
            _gridSize = gridSize;
            _start = start;
            _maxRoomSize = maxRoomSize;
            _maxSteps = maxSteps;
        }

        public List<Vector2> Execute()
        {
            Random rand = new Random();
            List<Vector2> path = new List<Vector2>();

            List<Rect> rects = new List<Rect>();
            List<Rect> corridoors = new List<Rect>();

            while (rects.Count < _maxSteps)
            {
                // create a rect
                int width = rand.Next(1, (int)_maxRoomSize.X);
                int height = rand.Next(1, (int)_maxRoomSize.Y);
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
                        var corridoor = CreateCorridoor(rect, rects[rects.Count - 1]);
                        corridoors.AddRange(corridoor);
                        foreach (Rect c in corridoor)
                        {
                            path.AddRange(c.ToList());
                        }
                    }
                    rects.Add(rect);
                }
            }
            // fill empty space with chambers
            Rect chamber = new Rect(0, 0, (int)_maxRoomSize.X, (int)_maxRoomSize.Y);
            bool placeable = false;
            List<Rect> blockages = new List<Rect>();
            blockages.AddRange(corridoors);
            blockages.AddRange(rects);

            for (int x = 1; x < _gridSize.X; x++)
            {
                for (int y = 1; y < _gridSize.Y; y++)
                {
                    Rect potentialChamber = new Rect(x, y, (int)_maxRoomSize.X, (int)_maxRoomSize.Y);
                    Rect margin = potentialChamber.Expand(1);
                    Godot.GD.Print(potentialChamber.Start.X);
                    foreach (Rect b in blockages)
                    {
                        if (margin.Intersects(b) || margin.End.X > _gridSize.X || margin.End.Y > _gridSize.Y)
                        {
                            placeable = false;
                            break;
                        }
                        else
                        {
                            placeable = true;
                        }
                    }
                    if (placeable)
                    {
                        chamber = new Rect(potentialChamber.Start, potentialChamber.Size);
                        break;
                    }
                }
                if (placeable) break;
            }
            if (placeable)
            {
                _rooms.Add(chamber);
                path.AddRange(chamber.ToList());
                rects.Add(chamber);
            }

            return path;
        }
        private List<Rect> CreateCorridoor(Rect r1, Rect r2)
        {

            Random rand = new Random();

            // randomly move vertical or horizontal first
            if (rand.Next(1, 2) == 1)
            {
                // horizontal
                var targetVec = new Vector2(r2.Mid.X, r1.Mid.Y);
                int x1 = (int)Math.Min(r1.Mid.X, targetVec.X);
                int x2 = (int)Math.Max(r1.Mid.X, targetVec.X);
                int width = x2 - x1 + 1;
                int height = 1;
                Rect rect1 = new Rect(x1, (int)r1.Mid.Y, width, height);


                // vertical
                int y1 = (int)Math.Min(targetVec.Y, r2.Mid.Y);
                int y2 = (int)Math.Max(targetVec.Y, r2.Mid.Y);
                int width2 = 1;
                int height2 = y2 - y1 + 1;
                Rect rect2 = new Rect((int)targetVec.X, y1, width2, height2);
                return new List<Rect>() { rect1, rect2 };
            }
            else
            {
                // vertical
                var targetVec = new Vector2(r1.Mid.X, r2.Mid.Y);
                int y1 = (int)Math.Min(r1.Mid.Y, targetVec.Y);
                int y2 = (int)Math.Max(r1.Mid.Y, targetVec.Y);
                int width = 1;
                int height = y2 - y1;
                Rect rect1 = new Rect((int)r1.Mid.X, y1, width, height);

                // horrizontal
                int x1 = (int)Math.Min(targetVec.X, r2.Mid.X);
                int x2 = (int)Math.Max(targetVec.X, r2.Mid.X);
                int width2 = x2 - x1;
                int height2 = 1;
                Rect rect2 = new Rect(x1, (int)targetVec.Y, width2, height2);
                return new List<Rect>() { rect1, rect2 };
            }
        }
        public List<Rect> GetRooms()
        {
            return _rooms;
        }

    }
}

