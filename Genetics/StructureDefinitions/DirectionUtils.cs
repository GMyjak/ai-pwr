using System;

namespace Genetics.StructureDefinitions
{
    public abstract class DirectionUtils
    {
        public static Direction GetOpposite(Direction d)
        {
            switch (d)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
            }

            throw new Exception("This exception should never be thrown XD");
        }

        public static bool Parallel(Direction d1, Direction d2)
        {
            return d1 == d2 || d1 == GetOpposite(d2);
        }
    }
}