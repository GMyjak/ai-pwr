using System;
using System.Collections.Generic;
using System.Linq;

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

    public enum SelectionType
    {
        Tournament,
        Roulette
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

        public Path Copy()
        {
            Path copy = new Path()
            {
                Segments = new List<Segment>(this.Segments)
            };

            return copy;
        }
    }

    public class Individual
    {
        // TODO move these to config
        //public static float crossPenalty = 90.0f;
        //public static float pathOutsideBoardPenalty = 90.0f;
        //public static float pathOutsideBoardPenaltyWeight = 10.4f;
        //public static float segmentNumberWeight = 0.8f;
        //public static float pathLengthWeight = 0.15f;
        public static float crossPenalty =10f;
        public static float pathOutsideBoardPenalty = 50.0f;
        public static float pathOutsideBoardPenaltyWeight = 15f;
        public static float segmentNumberWeight = 0.25f;
        public static float pathLengthWeight = 0.05f;
        public static int tournamentSize = 8;

        public Problem Problem { get; set; }
        public List<Path> Paths { get; set; }
        public float Score { get; set; }
        public float Penalty { get; set; }

        public Individual Copy()
        {
            Individual copy = new Individual()
            {
                Problem = this.Problem,
                Score = this.Score,
                Paths = new List<Path>()
            };
            Paths.ForEach(p => copy.Paths.Add(p.Copy()));

            return copy;
        }

        #region IndividualEvaluation
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

        public void AdaptToPopulation(float minimalPenalty)
        {
            Score = minimalPenalty / Penalty;
        }

        #endregion

        #region Mutations
        public void Mutate()
        {
            Random rng = UniversalRandom.Rng;
            int pathIndex = rng.Next() % Paths.Count;
            int segmentIndex = rng.Next() % Paths[pathIndex].Segments.Count;
            Direction direction = (Direction)(rng.Next() % 4);
            int pivot = rng.Next() % (Paths[pathIndex].Segments[segmentIndex].Length);
            int divideSegmentIndex = rng.Next() % 2;
            int offset = 1;
            offset = rng.NextDouble() > 0.80 ? 2 : offset;
            offset = rng.NextDouble() > 0.90 ? 3 : offset;
            offset = rng.NextDouble() > 0.95 ? 4 : offset;
            offset = rng.NextDouble() > 0.99 ? 5 : offset;


            MutateB(pathIndex, segmentIndex, direction, pivot, divideSegmentIndex, offset);
        }

        public bool MutateA(int pathIndex, int segmentIndex, Direction direction)
        {
            return MutateB(pathIndex, segmentIndex, direction, 0, 0);
        }

        public bool MutateB(int pathIndex, int segmentIndex, Direction direction, int pivot, int dividedSegmentIndex, int offset = 1)
        {
            #region InitialConditions
            if (pathIndex < 0 || pathIndex >= Paths.Count)
            {
                return false;
            }

            if (segmentIndex < 0 || segmentIndex >= Paths[pathIndex].Segments.Count)
            {
                return false;
            }

            Segment s = Paths[pathIndex].Segments[segmentIndex];
            if (pivot < 0 || pivot > s.Length)
            {
                return false;
            }
            if (DirectionUtils.Parallel(s.Direction, direction))
            {
                return false;
            }
            if (dividedSegmentIndex != 0 && dividedSegmentIndex != 1)
            {
                return false;
            }
            #endregion

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

            workList.Insert(segmentIndex, new Segment(direction, offset));
            workList.Insert(segmentIndex + 2, new Segment(DirectionUtils.GetOpposite(direction), offset));

            FixSegments(pathIndex);

            return true;
        }

        private void FixSegments(int pathIndex)
        {
            // TODO żeby nie była upośledzona
            Path path = Paths[pathIndex];
            for (int i = 0; i < path.Segments.Count - 1; i++)
            {
                while (i < path.Segments.Count - 1 && DirectionUtils.Parallel(path.Segments[i].Direction, path.Segments[i + 1].Direction))
                {
                    Segment? merged = path.Segments[i].Merge(path.Segments[i + 1]);
                    path.Segments.RemoveAt(i);
                    path.Segments.RemoveAt(i);
                    if (merged != null)
                    {
                        path.Segments.Insert(i, merged.Value);
                    }
                    else
                    {
                        i = Math.Max(i - 1, 0);
                    }
                }
            }
        }

        #endregion

        #region Crossing
        public static Individual Cross(Individual a, Individual b)
        {
            Random rng = UniversalRandom.Rng;
            int pivot = rng.Next() % a.Paths.Count;

            List<Path> newList = new List<Path>();

            for (int i = 0; i < pivot; i++)
            {
                newList.Add(a.Paths[i]);
            }

            for (int i = pivot; i < a.Paths.Count; i++)
            {
                newList.Add(b.Paths[i]);
            }

            return new Individual { Paths = newList, Problem = a.Problem, Score = 0 }; ;
        }
        #endregion

        #region Selection
        public static Individual Select(List<Individual> population, SelectionType selectionType)
        {
            if (selectionType == SelectionType.Tournament)
            {
                return SelectTournament(population);
            }
            else if (selectionType == SelectionType.Roulette)
            {
                return SelectRoulette(population);
            }

            throw new Exception("Invalid selection method");
        }

        private static Individual SelectTournament(List<Individual> population)
        {
            Random rng = UniversalRandom.Rng;
            List<Individual> temp = new List<Individual>(population);
            var result = temp.OrderBy(ind => rng.Next())
                .Take(tournamentSize)
                .OrderBy(ind => ind.Penalty)
                .First();

            return result;
        }

        private static Individual SelectRoulette(List<Individual> population)
        {
            Random rng = UniversalRandom.Rng;
            float sum = population.Sum(i => i.Score);
            float aggregate = 0;
            float shot = (float)rng.NextDouble() * sum;

            foreach (var individual in population)
            {
                aggregate += individual.Score;
                if (aggregate > shot)
                {
                    return individual;
                }
            }

            return population.Last();
        }

        #endregion
    }
}
