using System.Numerics;
using System;
using System.Collections.Generic;

namespace GamePasta.DungeonAlgorythms
{
    public enum Direction
    {
        NORTH, SOUTH, EAST, WEST
    }

    public interface IRandomAlgorythm
    {
        List<Vector2> Execute();
    }

    public class RandomWalk : IRandomAlgorythm
    {
        private Vector2 _gridSize;
        private Vector2 _start;
        private List<Vector2> _mask;
        private int _maxSteps;
        private Direction _direction;
        private Dictionary<Direction, Vector2> _directionMap;
        public RandomWalk(Vector2 gridSize, Vector2 start, List<Vector2> mask, Direction direction, int maxSteps)
        {
            _gridSize = gridSize;
            _start = start;
            _mask = mask;
            _maxSteps = maxSteps;
            _direction = direction;
            _directionMap = new Dictionary<Direction, Vector2>()
            {
                {Direction.NORTH, new Vector2(0, -1)},
                {Direction.SOUTH, new Vector2(0, 1)},
                {Direction.EAST, new Vector2(-1, 0)},
                {Direction.WEST, new Vector2(1, 0)}
            };
        }
        public List<Vector2> Execute()
        {
            Random rand = new Random();
            Vector2 current = _start;
            List<Vector2> path = new List<Vector2>();
            int steps = 0;

            while (true)
            {
                if (_maxSteps > 0 && steps >= _maxSteps) break;

                List<Vector2> neighbours = new List<Vector2>();
                path.Add(current);

                foreach (Vector2 vec in GetPotentialNeighbours(current))
                {
                    if (NotOutOfBounds(vec) && !path.Contains(vec) && !_mask.Contains(vec) && NotInWrongDirection(vec, current))
                    {
                        neighbours.Add(vec);
                    }
                }

                if (neighbours.Count <= 1)
                {
                    // We may be at the end of our path
                    if (current.X + _directionMap[_direction].X > _gridSize.X &&
                        current.Y + _directionMap[_direction].Y > _gridSize.Y)
                    {
                        // we signify the exit with a negative vec2
                        neighbours.Add(new Vector2(-1, -1));
                    }
                }
                if (neighbours.Count > 0)
                {
                    current = neighbours[rand.Next(0, neighbours.Count)];

                    // check if exit
                    if (current.X < 0 && current.Y < 0)
                    {
                        break;
                    }
                }
                else break;
                steps++;
            }

            return path;
        }

        private bool NotInWrongDirection(Vector2 vec, Vector2 current)
        {
            return (vec != new Vector2(current.X + _directionMap[_direction].X * -1, current.Y + _directionMap[_direction].Y * -1));
        }

        private bool NotOutOfBounds(Vector2 vec)
        {
            bool notUnderBounds = (vec.X >= 0 && vec.Y >= 0);
            bool notOverbounds = (vec.X < _gridSize.X - 1 && vec.Y < _gridSize.Y - 1);
            return (notOverbounds && notUnderBounds);
        }

        private IEnumerable<Vector2> GetPotentialNeighbours(Vector2 current)
        {
            List<Vector2> neighbours = new List<Vector2>()
            {
                new Vector2(current.X, current.Y - 1),
                new Vector2(current.X - 1, current.Y),
                new Vector2(current.X, current.Y + 1),
                new Vector2(current.X + 1, current.Y)
            };
            return neighbours;
        }
    }
}

