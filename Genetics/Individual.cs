using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.StructureDefinitions;

namespace Genetics
{
    public class Individual
    {
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
                initialScore += path.Segments.Count * Config.segmentNumberWeight;
                initialScore += path.Segments.Sum(seg => seg.Length * Config.pathLengthWeight);
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
                            initialScore += Config.crossPenalty;
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
                        initialScore += atomsOutside * Config.pathOutsideBoardPenaltyWeight;
                        if (firstTimeOutside)
                        {
                            firstTimeOutside = false;
                            initialScore += Config.pathOutsideBoardPenalty;
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
            int pivot1 = rng.Next() % Math.Max(Paths[pathIndex].Segments[segmentIndex].Length, 1);
            int pivot2 = rng.Next() % Math.Max(Paths[pathIndex].Segments[segmentIndex].Length, 1);
            int divideSegmentIndex = rng.Next() % 2;
            int offset = 1;
            offset = rng.NextDouble() > 0.70 ? 2 : offset;
            offset = rng.NextDouble() > 0.78 ? 3 : offset;
            offset = rng.NextDouble() > 0.84 ? 4 : offset;
            offset = rng.NextDouble() > 0.88 ? 5 : offset;
            if (rng.NextDouble() > 0.91)
            {
                if (direction == Direction.Up || direction == Direction.Down)
                {
                    offset = rng.Next() % (Math.Max(Problem.Dimensions.Y - 6, 1) + 6);
                }
                else
                {
                    offset = rng.Next() % (Math.Max(Problem.Dimensions.X - 6, 1) + 6);
                }
            }

            if (rng.NextDouble() < 0.4)
            {
                MutateA(pathIndex, segmentIndex, direction);
            }
            else if (rng.NextDouble() < 0.8)
            {
                MutateB(pathIndex, segmentIndex, direction, pivot1, divideSegmentIndex, offset);
            }
            else if (pivot1 != pivot2)
            {
                MutateC(pathIndex, segmentIndex, direction, Math.Min(pivot1, pivot2), 
                    Math.Max(pivot1, pivot2), offset);
            }
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

        public bool MutateC(int pathIndex, int segmentIndex, Direction direction, int pivotA, int pivotB, int offset = 1)
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
            if (pivotA < 0 || pivotA > s.Length || pivotB < 0 || pivotB > s.Length)
            {
                return false;
            }
            if (DirectionUtils.Parallel(s.Direction, direction))
            {
                return false;
            }
            #endregion

            List<Segment> workList = Paths[pathIndex].Segments;

            // Divide into two segments if necessary
            if (pivotA > 0 && pivotA < workList[segmentIndex].Length && pivotB > 0 && pivotB < workList[segmentIndex].Length && pivotA < pivotB)
            {
                Segment segment1 = new Segment(workList[segmentIndex].Direction, pivotA);
                Segment segment2 = new Segment(workList[segmentIndex].Direction, pivotB - pivotA);
                Segment segment3 =
                    new Segment(workList[segmentIndex].Direction, workList[segmentIndex].Length - pivotB);
                workList.RemoveAt(segmentIndex);
                workList.Insert(segmentIndex, segment3);
                workList.Insert(segmentIndex, segment2);
                workList.Insert(segmentIndex, segment1);
                segmentIndex += 1;
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
                .Take(Config.tournamentSize)
                .OrderBy(ind => ind.Penalty)
                .First();

            return result;
        }

        private static Individual SelectRoulette(List<Individual> population)
        {
            Random rng = UniversalRandom.Rng;
            float sum = population.Sum(i => i.Score);
            float aggregate = 0;
            double shot = rng.NextDouble() * sum;

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
