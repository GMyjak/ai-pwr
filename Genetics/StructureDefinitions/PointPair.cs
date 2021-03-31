namespace Genetics.StructureDefinitions
{
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