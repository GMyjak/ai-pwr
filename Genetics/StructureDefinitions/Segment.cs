using System;

namespace Genetics.StructureDefinitions
{
    public struct Segment
    {
        public Direction Direction { get; set; }
        public int Length { get; set; }

        public Segment(Direction d, int l)
        {
            Direction = d;
            Length = l;
        }

        public Segment? Merge(Segment other)
        {
            if (!DirectionUtils.Parallel(Direction, other.Direction))
            {
                throw new Exception("Segments cannot be merged");
            }

            int newLength = Length;
            if (Direction == other.Direction)
            {
                newLength += other.Length;
            }
            else
            {
                newLength -= other.Length;
            }

            Segment result = new Segment(newLength > 0 ? Direction : other.Direction, Math.Abs(newLength));

            if (result.Length == 0)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
    }
}