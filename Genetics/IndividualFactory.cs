using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.StructureDefinitions;

namespace Genetics
{
    public abstract class IndividualFactory
    {
        public static List<Individual> GenerateByConnectingPoints(Problem problem, int resultCount)
        {
            if (resultCount < 1)
            {
                throw new Exception("You cannot generate less tan one individual");
            }

            var results = new List<Individual>();
            var rng = UniversalRandom.Rng;

            for (int i = 0; i < resultCount; i++)
            {
                var result = new Individual 
                {
                    Problem = problem, 
                    Paths = new List<Path>()
                };

                foreach (var points in problem.ConnectedPoints)
                {
                    int horizontalOffset = points.B.X - points.A.X;
                    var horizontal = new Segment(horizontalOffset > 0 ? Direction.Right : Direction.Left, Math.Abs(horizontalOffset));

                    int verticalOffset = points.B.Y - points.A.Y;
                    var vertical = new Segment(verticalOffset > 0 ? Direction.Up : Direction.Down, Math.Abs(verticalOffset));

                    var path = new Path
                    {
                        Segments = new List<Segment>()
                    };
                    if (rng.Next() % 2 == 0)
                    {
                        if (vertical.Length > 0) path.Segments.Add(vertical);
                        if (horizontal.Length > 0) path.Segments.Add(horizontal);
                    }
                    else
                    {
                        if (horizontal.Length > 0) path.Segments.Add(horizontal);
                        if (vertical.Length > 0) path.Segments.Add(vertical);
                    }
                    result.Paths.Add(path);
                }

                results.Add(result);
            }

            results.ForEach(ind =>
            {
                ind.Mutate();
                ind.Mutate();
                ind.Mutate();
                ind.Mutate();
            });
            return results;
        }

        public static List<Individual> GenerateByPickingBest(Problem problem, int resultCount)
        {
            List<Individual> result = GenerateByConnectingPoints(problem, 1000000);
            result.ForEach(ind => ind.Penalty = ind.Evaluate());
            return result.OrderBy(ind => ind.Penalty).Take(resultCount).ToList();
        }
    }
}
