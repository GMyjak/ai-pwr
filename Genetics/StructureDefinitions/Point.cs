using System;

namespace Genetics.StructureDefinitions
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(string sx, string sy)
        {
            X = int.Parse(sx);
            Y = int.Parse(sy);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public Point Append(int len, Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return new Point(X, Y-len);
                case Direction.Up:
                    return new Point(X, Y+len);
                case Direction.Left:
                    return new Point(X-len, Y);
                case Direction.Right:
                    return new Point(X+len, Y);
            }

            throw new Exception();
        }

        public static int GetAtomsOutsideBounds(Point a, Point b, int maxX, int maxY)
        {
            // Assume that line is horizontal or vertical

            if (a.IsInsideBounds(maxX, maxY) && b.IsInsideBounds(maxX, maxY))
            {
                return 0;
            }
            else if (!a.IsInsideBounds(maxX, maxY) && !b.IsInsideBounds(maxX, maxY))
            {
                return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            }
            else
            {
                Point inside = a.IsInsideBounds(maxX, maxY) ? a : b;
                Point outside = a.IsInsideBounds(maxX, maxY) ? b : a;
                if (inside.X == outside.X)
                {
                    return Math.Min(Math.Abs(outside.Y - maxY), Math.Abs(outside.Y));
                }
                else
                {
                    return Math.Min(Math.Abs(outside.X - maxX), Math.Abs(outside.X));
                }
            }
        }

        public bool IsInsideBounds(int maxX, int maxY)
        {
            return X >= 0 && X < maxX && Y >= 0 && Y < maxY;
        }
    }
}