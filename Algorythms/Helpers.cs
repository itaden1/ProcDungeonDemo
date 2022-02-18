using System.Numerics;
using System.Collections.Generic;
using System;

namespace GamePasta.DungeonAlgorythms
{
    public class Helpers
    {
        public static List<Rect> CreateCorridoor(Rect r1, Rect r2)
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
                int height = y2 - y1 + 1;
                Rect rect1 = new Rect((int)r1.Mid.X, y1, width, height);

                // horrizontal
                int x1 = (int)Math.Min(targetVec.X, r2.Mid.X);
                int x2 = (int)Math.Max(targetVec.X, r2.Mid.X);
                int width2 = x2 - x1;
                int height2 = 1 + 1;
                Rect rect2 = new Rect(x1, (int)targetVec.Y, width2, height2);
                return new List<Rect>() { rect1, rect2 };
            }
        }
        public static System.Numerics.Vector2 GetClosestVector(System.Numerics.Vector2 vec, List<System.Numerics.Vector2> vectors)
        {
            if (vectors.Count <= 0) return vec;
            System.Numerics.Vector2 closest = vectors[0];
            float oldDist = 1000000;
            foreach (System.Numerics.Vector2 vector in vectors)
            {
                var dist = System.Numerics.Vector2.Distance(vec, vector);
                if (dist < oldDist)
                {
                    closest = vector;
                    oldDist = dist;
                }
            }
            return closest;
        }
        public static byte getFourBitMask(List<Vector2> otherVecs, Vector2 vec)
        {

            Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>
            {
                {1, new Vector2(vec.X, vec.Y-1)},
                {2, new Vector2(vec.X-1, vec.Y)},
                {4, new Vector2(vec.X+1, vec.Y)},
                {8, new Vector2(vec.X, vec.Y+1)}
            };

            byte total = 0;

            foreach (var pos in positions)
            {
                bool add = !otherVecs.Contains(pos.Value);

                if (add) total += (byte)pos.Key;
            }
            return total;
        }
        public static List<Vector2> GetNeighbours(Vector2 vec)
        {
            return new List<Vector2>()
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
        }
    }

}

