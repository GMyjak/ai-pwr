using System;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem p = ProblemLoader.LoadProblemFromFile("../../../TestData/zad1.txt");
            
            TestGenerator(p);

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
    }
}
