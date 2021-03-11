using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetics
{
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3
    }

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

    public struct Path
    {
        public List<Segment> Segments { get; set; }
    }

    public class Individual
    {
        // TODO move these to config
        public static float crossPenalty = 9.0f;
        public static float pathOutsideBoardPenalty = 9.0f;
        public static float pathOutsideBoardPenaltyWeight = 1.4f;
        public static float segmentNumberWeight = 0.8f;
        public static float pathLengthWeight = 0.15f;

        public Problem Problem { get; set; }
        public List<Path> Paths { get; set; }

        // Fitness function
        public float Evaluate()
        {
            float initialScore = 0;

            // Base score
            foreach (var path in Paths)
            {
                initialScore += path.Segments.Count * segmentNumberWeight;
                initialScore += path.Segments.Sum(seg => seg.Length * pathLengthWeight);
            }

            // For each path calculate exact points
            List<List<PointPair>> allSegmentPoints = new List<List<PointPair>>();
            for (int i = 0; i < Paths.Count; i++)
            {
                Point current = Problem.ConnectedPoints[i].A;
                Point next;
                List<Segment> workingList = Paths[i].Segments;
                List<PointPair> pointsForPath = new List<PointPair>();

                foreach (var segment in workingList)
                {
                    next = current;
                    switch (segment.Direction)
                    {
                        case Direction.Up: next.Y += segment.Length; break;
                        case Direction.Down: next.Y -= segment.Length; break;
                        case Direction.Left: next.X -= segment.Length; break;
                        case Direction.Right: next.X += segment.Length; break;
                    }
                    pointsForPath.Add(new PointPair(current, next));
                    current = next;
                }

                allSegmentPoints.Add(pointsForPath);
            }

            // Cross penalty
            for (int i = 0; i < allSegmentPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < allSegmentPoints.Count; j++)
                {
                    allSegmentPoints[i].ForEach(seg1 => allSegmentPoints[j].ForEach(seg2 =>
                    {
                        if (seg1.Crosses(seg2))
                        {
                            initialScore += crossPenalty;
                        }
                    }));
                }
            }

            // Path outside penalty
            
            foreach (var innerList in allSegmentPoints)
            {
                bool firstTimeOutside = true;
                foreach (var pointPair in innerList)
                {
                    int atomsOutside = Point.GetAtomsOutsideBounds(pointPair.A, pointPair.B, Problem.Dimensions.X, Problem.Dimensions.Y);

                    if (atomsOutside > 0)
                    {
                        initialScore += atomsOutside * pathOutsideBoardPenaltyWeight;
                        if (firstTimeOutside)
                        {
                            firstTimeOutside = false;
                            initialScore += pathOutsideBoardPenalty;
                        }
                    }
                }
            }

            return initialScore;
        }

        #region Mutations
        public bool MutateA(int pathIndex, int segmentIndex, Direction direction)
        {
            return MutateB(pathIndex, segmentIndex, direction, 0, 0);
        }

        public bool MutateB(int pathIndex, int segmentIndex, Direction direction, int pivot, int dividedSegmentIndex)
        {
            if (pathIndex < 0 || pathIndex >= Paths.Count)
            {
                return false;
            }

            if (segmentIndex < 0 || segmentIndex >= Paths[pathIndex].Segments.Count)
            {
                return false;
            }
            else
            {
                Segment s = Paths[pathIndex].Segments[segmentIndex];

                if (pivot < 0 || pivot > s.Length)
                {
                    return false;
                }

                List<Direction> vertical = new List<Direction>() { Direction.Up, Direction.Down };
                List<Direction> horizontal = new List<Direction>() { Direction.Left, Direction.Right };

                if (vertical.Contains(s.Direction) && vertical.Contains(direction))
                {
                    return false;
                }
                else if (horizontal.Contains(s.Direction) && horizontal.Contains(direction))
                {
                    return false;
                }
            }

            if (dividedSegmentIndex != 0 && dividedSegmentIndex != 1)
            {
                return false;
            }

            List<Segment> workList = Paths[pathIndex].Segments;

            // Divide into two segments if necessary
            if (pivot > 0 && pivot < workList[segmentIndex].Length)
            {
                Segment updatedSegment = new Segment(workList[segmentIndex].Direction, pivot);
                Segment newSegment =
                    new Segment(workList[segmentIndex].Direction, workList[segmentIndex].Length - pivot);
                workList.RemoveAt(segmentIndex);
                workList.Insert(segmentIndex, newSegment);
                workList.Insert(segmentIndex, updatedSegment);
                segmentIndex += dividedSegmentIndex;
            }

            // Now we just have to consider changes according to A type mutation
            // If it is first segment or segment has been split we have to add dummy segment with length 0
            if (segmentIndex == 0 || workList[segmentIndex - 1].Direction == workList[segmentIndex].Direction)
            {
                workList.Insert(segmentIndex, new Segment(direction, 0));
                segmentIndex++;
            }
            if (segmentIndex == workList.Count - 1 || workList[segmentIndex + 1].Direction == workList[segmentIndex].Direction)
            {
                workList.Insert(segmentIndex, new Segment(direction, 0));
            }
            // Update segment before
            int updatedLengthBefore = workList[segmentIndex - 1].Length;
            if (workList[segmentIndex - 1].Direction == direction)
            {
                updatedLengthBefore++;
            }
            else
            {
                updatedLengthBefore--;
            }

            if (updatedLengthBefore > 0)
            {
                Segment newSegment = new Segment(workList[segmentIndex - 1].Direction, updatedLengthBefore);
                workList.RemoveAt(segmentIndex - 1);
                workList.Insert(segmentIndex, newSegment);
            }
            // Merge updated segment with segment before
            else
            {
                workList.RemoveAt(segmentIndex - 1);
                Segment? merged = workList[segmentIndex - 1].Merge(workList[segmentIndex]);
                if (merged != null)
                {
                    workList.Insert(segmentIndex - 1, merged.Value);
                    workList.RemoveAt(segmentIndex);
                    workList.RemoveAt(segmentIndex);
                    segmentIndex--;
                }
                else
                {
                    segmentIndex--;
                    workList.RemoveAt(segmentIndex);
                    workList.RemoveAt(segmentIndex);
                    segmentIndex--;
                }

            }
            // TODO handle second part

            return true;
        }
        #endregion
    }
}
