using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Genetics.StructureDefinitions;

namespace Genetics
{
    class Program
    {
        private static string problem0 = "../../../TestData/zad0.txt";
        private static string problem1 = "../../../TestData/zad1.txt";
        private static string problem2 = "../../../TestData/zad2.txt";
        private static string problem3 = "../../../TestData/zad3.txt";

        private static Problem p0 = ProblemLoader.LoadProblemFromFile(problem0);
        private static Problem p1 = ProblemLoader.LoadProblemFromFile(problem1);
        private static Problem p2 = ProblemLoader.LoadProblemFromFile(problem2);
        private static Problem p3 = ProblemLoader.LoadProblemFromFile(problem3);

        static void Main(string[] args)
        {
            UniversalRandom.Seed = new Random().Next();

            Config.Reset();
            Console.WriteLine("P1");
            TestPopulationSize(p1);
            Console.WriteLine("P2");
            TestPopulationSize(p2);
            Console.WriteLine("P3");
            TestPopulationSize(p3);

            Config.Reset();
            Console.WriteLine("P1");
            TestMutationProb(p1);
            Console.WriteLine("P2");
            TestMutationProb(p2);
            Console.WriteLine("P3");
            TestMutationProb(p3);

            Config.Reset();
            Console.WriteLine("P1");
            TestCrossProb(p1);
            Console.WriteLine("P2");
            TestCrossProb(p2);
            Console.WriteLine("P3");
            TestCrossProb(p3);

            Config.Reset();
            Console.WriteLine("P1");
            TestSelectionType(p1);
            Console.WriteLine("P2");
            TestSelectionType(p2);
            Console.WriteLine("P3");
            TestSelectionType(p3);

            Config.Reset();
            TestRandomMethod(p1);
            TestRandomMethod(p2);
            TestRandomMethod(p3);

            Config.Reset();
            TestGA(p1);
            TestGA(p2);
            TestGA(p3);
        }

        static void TestGA(Problem p)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm()
            {
                Problem = p,
                InitFactoryMethod = IndividualFactory.GenerateByConnectingPoints,
            };

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            long lastTime = 0;
            Individual overallBest = null;

            ga.OnIterationFinished += (idx, pop) =>
            {
                float minPenalty = pop.Min(i => i.Penalty);
                Individual best = pop.First(i => i.Penalty == minPenalty);
                

                if (idx % 10 == 0 || idx == Config.generationCount - 1)
                {
                    Console.WriteLine("Generation: " + idx); 
                    Console.WriteLine("Min: " + minPenalty);
                    Console.WriteLine("Time: " + sw.ElapsedMilliseconds / 1000 + "." + sw.ElapsedMilliseconds % 1000);
                    long deltaTime = sw.ElapsedMilliseconds - lastTime;
                    lastTime = sw.ElapsedMilliseconds;
                    Console.WriteLine("Delta time: " + deltaTime / 1000 + "." + deltaTime % 1000 + "\n");
                    overallBest = best;
                }
            };

            ga.Run();
            sw.Stop();

            Picture pic = new Picture(overallBest);
            pic.Draw();
            pic.Save("best.bmp");
        }

        static void TestRandomMethod(Problem p)
        {
            int individualCount = 1000000;

            List<Individual> subjects = IndividualFactory.GenerateByConnectingPoints(p, individualCount);
            subjects.ForEach(s =>
            {
                s.Penalty = s.Evaluate();
            });

            Console.Write(subjects.Min(s => s.Penalty) + ", ");
            Console.Write(subjects.Max(s => s.Penalty) + ", ");
            float avg = subjects.Average(s => s.Penalty);
            var sum = subjects.Sum(s => (s.Penalty - avg) * (s.Penalty - avg));
            var std = Math.Sqrt(sum / subjects.Count);

            Console.Write(avg + ", ");
            Console.WriteLine(std);
        }

        static void TestPopulationSize(Problem p)
        {
            Config.Reset();
            Console.WriteLine("Population: 100");
            Config.populationSize = 100;
            TestInstance(p, "population");
            Console.WriteLine("Population: 200");
            Config.populationSize = 200;
            TestInstance(p, "population");
            Console.WriteLine("Population: 300");
            Config.populationSize = 300;
            TestInstance(p, "population");
            Console.WriteLine("Population: 400");
            Config.populationSize = 400;
            TestInstance(p, "population");
            Console.WriteLine("Population: 500");
            Config.populationSize = 500;
            TestInstance(p, "population");
        }

        static void TestMutationProb(Problem p)
        {
            Config.Reset();
            Console.WriteLine("Mutation: 0.2");
            Config.mutationProbability = 0.2f;
            TestInstance(p, "mut");
            Console.WriteLine("Mutation: 0.4");
            Config.mutationProbability = 0.4f;
            TestInstance(p, "mut");
            Console.WriteLine("Mutation: 0.6");
            Config.mutationProbability = 0.6f;
            TestInstance(p, "mut");
            Console.WriteLine("Mutation: 0.8");
            Config.mutationProbability = 0.8f;
            TestInstance(p, "mut");
        }

        static void TestCrossProb(Problem p)
        {
            Config.Reset();
            Console.WriteLine("Cross: 0.2");
            Config.crossProbability = 0.2f;
            TestInstance(p, "crs");
            Console.WriteLine("Cross: 0.4");
            Config.crossProbability = 0.4f;
            TestInstance(p, "crs");
            Console.WriteLine("Cross: 0.6");
            Config.crossProbability = 0.6f;
            TestInstance(p, "crs");
            Console.WriteLine("Cross: 0.8");
            Config.crossProbability = 0.8f;
            TestInstance(p, "crs");
        }

        static void TestSelectionType(Problem p)
        {
            Config.Reset();
            Console.WriteLine("ROUL");
            Config.selectionType = SelectionType.Roulette;
            TestInstance(p, "sel");
            Console.WriteLine("TOUR7");
            Config.selectionType = SelectionType.Tournament;
            Config.tournamentSize = 7;
            TestInstance(p, "sel");
            Console.WriteLine("TOUR15");
            Config.selectionType = SelectionType.Tournament;
            Config.tournamentSize = 15;
            TestInstance(p, "sel");
            Console.WriteLine("TOUR40");
            Config.selectionType = SelectionType.Tournament;
            Config.tournamentSize = 40;
            TestInstance(p, "sel");
        }

        static void TestInstance(Problem p, string testType)
        {
            List<float> penalties = new List<float>();
            for (int i = 0; i < 10; i++)
            {
                Console.Write(i);
                GeneticAlgorithm ga = GetGA(p);
                penalties.Add(ga.Run().Penalty);
            }
            Console.WriteLine();

            Console.WriteLine("Results:");
            Console.Write(penalties.Min() + ", ");
            Console.Write(penalties.Max() + ", ");
            var avg = penalties.Average();
            Console.Write(avg + ", ");

            var sum = penalties.Sum(d => (d - avg) * (d - avg));
            var std = Math.Sqrt(sum / penalties.Count);
            Console.Write(std + "\n");
        }

        static GeneticAlgorithm GetGA(Problem p)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm()
            {
                Problem = p,
                InitFactoryMethod = IndividualFactory.GenerateByConnectingPoints,
            };

            return ga;
        }
    }
}
