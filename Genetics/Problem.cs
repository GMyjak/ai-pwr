using System;
using System.Collections.Generic;
using System.Text;

namespace Genetics
{
    public class Problem
    {
        public Point Dimensions { get; set; }
        public List<PointPair> ConnectedPoints { get; set; }
    }

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
            return X >= 0 && X <= maxX && Y >= 0 && Y <= maxY;
        }
    }

    public struct PointPair
    {
        public Point A { get; set; }
        public Point B { get; set; }

        public PointPair(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public bool Crosses(PointPair other)
        {
            // Assume that line is horizontal or vertical
            // Crosses of L or T shape are also handled

            // Both vertical
            if (A.X == B.X && A.X == other.A.X && A.X == other.B.X)
            {
                if (A.Y >= other.A.Y && other.A.Y >= B.Y)
                {
                    return true;
                }
                else if (A.Y <= other.A.Y && other.A.Y <= B.Y)
                {
                    return true;
                }
                else if (A.Y >= other.B.Y && other.B.Y >= B.Y)
                {
                    return true;
                }
                else if (A.Y <= other.B.Y && other.B.Y <= B.Y)
                {
                    return true;
                }
            }
            //Both horizontal
            else if (A.Y == B.Y && A.Y == other.A.Y && A.Y == other.B.Y)
            {
                if (A.X >= other.A.X && other.A.X >= B.X)
                {
                    return true;
                }
                else if (A.X <= other.A.X && other.A.X <= B.X)
                {
                    return true;
                }
                else if (A.X >= other.B.X && other.B.X >= B.X)
                {
                    return true;
                }
                else if (A.X <= other.B.X && other.B.X <= B.X)
                {
                    return true;
                }
            }
            // In this case one line has to be horizontal and one vertical (look at cases above)
            else
            {
                PointPair vertical = A.X == B.X ? this : other;
                PointPair horizontal = A.Y == B.Y ? this : other;

                if (vertical.A.Y >= horizontal.A.Y && horizontal.A.Y >= vertical.B.Y)
                {
                    if (horizontal.A.X >= vertical.A.X && vertical.A.X >= horizontal.B.X)
                    {
                        return true;
                    }
                    else if (horizontal.A.X <= vertical.A.X && vertical.A.X <= horizontal.B.X)
                    {
                        return true;
                    }
                }
                else if (vertical.A.Y <= horizontal.A.Y && horizontal.A.Y <= vertical.B.Y)
                {
                    if (horizontal.A.X >= vertical.A.X && vertical.A.X >= horizontal.B.X)
                    {
                        return true;
                    }
                    else if (horizontal.A.X <= vertical.A.X && vertical.A.X <= horizontal.B.X)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Touches(PointPair other)
        {
            return A == other.A || A == other.B || B == other.A || B == other.B;
        }
    }
}
