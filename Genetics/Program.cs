using System;
using System.Linq;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem p = ProblemLoader.LoadProblemFromFile("../../../TestData/zad2.txt");
            
            TestGA(p);

            //TestGenerator(p);
        }

        static void TestGenerator(Problem p)
        {
            var individuals = IndividualFactory.GenerateByConnectingPoints(p, 3);
            Console.WriteLine("Individuals generated");

            foreach (var individual in individuals)
            {
                Console.WriteLine(individual.Evaluate());
            }
        }

        static void TestGA(Problem p)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm()
            {
                Problem = p,
                CrossProbability = 0.2f,
                GenerationCount = 10000,
                InitFactoryMethod = IndividualFactory.GenerateByConnectingPoints,
                MutationProbability = 0.1f,
                PopulationSize = 70
            };

            ga.OnIterationFinished += (idx, pop) =>
            {
                Console.WriteLine("Generation: " + idx);
                //Console.WriteLine("Avg: " + pop.Average(ind => ind.Penalty));

                float minPenalty = pop.Min(i => i.Penalty);
                Individual best = pop.First(i => i.Penalty == minPenalty);
                Console.WriteLine("Min: " + minPenalty);

                if (idx == 1200)
                {
                    Console.WriteLine(best);
                }
            };

            ga.Run();
        }
    }
}
