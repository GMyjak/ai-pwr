using System;
using System.Collections.Generic;
using System.Text;

namespace Genetics
{
    public class Problem
    {
        public Point Dimensions { get; set; }
        public List<PointPair> Paths { get; set; }
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
    }
}
