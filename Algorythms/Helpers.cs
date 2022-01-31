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

    }
}

