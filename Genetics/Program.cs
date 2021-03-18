using System;
using System.Linq;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem p = ProblemLoader.LoadProblemFromFile("../../../TestData/zad1.txt");
            
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
                Console.WriteLine("Best individual: " + pop.Average(ind => ind.Penalty));

                if (idx == 0 || idx == 30 || idx == 70)
                {
                    Console.WriteLine("XD");
                }
            };

            ga.Run();
        }
    }
}
