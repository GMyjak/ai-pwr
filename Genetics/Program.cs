using System;

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
                CrossProbability = 0.8f,
                GenerationCount = 100,
                InitFactoryMethod = IndividualFactory.GenerateByConnectingPoints,
                MutationProbability = 0.2f,
                PopulationSize = 40
            };

            ga.OnIterationFinished += (idx, pop) =>
            {
                // todo
            };

            ga.Run();
        }
    }
}
