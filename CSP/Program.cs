using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSP.EinsteinRiddle;
using CSP.MapColouring;
using Variable = CSP.MapColouring.Variable;

namespace CSP
{
    class Program
    {
        static int sum = 0;

        static void Main(string[] args)
        {
            Random rng = new Random();
            int seed = rng.Next();
            //TestEinstein();
            //TestMCP(seed);
            //TestEinsteinFC();
            //TestMCPFC(seed);

            //List<int> seeds = new List<int>();
            //for (int i = 0; i < 6; i++)
            //{
            //    seeds.Add(rng.Next());
            //}

            //List<int> mapSizes = new List<int>() {3, 4,5,6,7,8,9};

            //foreach (var mapSize in mapSizes)
            //{
            //    Console.WriteLine("TESTS FOR MAP SIZE " + mapSize);
            //    float btCounter = 0;
            //    float fcCounter = 0;
            //    foreach (var i in seeds)
            //    {
            //        btCounter += TestMCP(i, mapSize);
            //        fcCounter += TestMCPFC(i, mapSize);
            //    }
            //    Console.WriteLine($"BT: {btCounter/seeds.Count} FC: {fcCounter/seeds.Count}");
            //}

            //List<int> seeds = new List<int>();
            //for (int i = 0; i < 6; i++)
            //{
            //    seeds.Add(rng.Next());
            //}

            //List<int> mapSizes = new List<int>() { 3, 4, 5, 6, 7, 8, 9 };

            //foreach (var mapSize in mapSizes)
            //{
            //    Console.WriteLine("TESTS FOR MAP SIZE " + mapSize);
            //    float btCounter = 0;
            //    float fcCounter = 0;
            //    foreach (var i in seeds)
            //    {
            //        Stopwatch sw = new Stopwatch();
            //        sw.Start();
            //        TestMCP(i, mapSize);
            //        sw.Stop();
            //        btCounter += sw.ElapsedMilliseconds;
            //        sw.Reset();
            //        sw.Start();
            //        TestMCPFC(i, mapSize);
            //        sw.Stop();
            //        fcCounter += sw.ElapsedMilliseconds;
            //    }
            //    Console.WriteLine($"BT: {btCounter / seeds.Count} FC: {fcCounter / seeds.Count}");
            //}

            //sum = 0;
            //foreach (var i in seeds)
            //{
            //    TestMCPFC(i);
            //}

            //Console.WriteLine((float)sum / 10);

            TestMCPFC(rng.Next(), 25);
        }

        static void TestEinstein()
        {
            int solutionCount = 0;
            EinsteinRiddleProblem riddle = new EinsteinRiddleProblem();
            riddle.Initialize();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;

                EinsteinRiddleProblem.DisplaySolution(xd);
            };
            riddle.RunBacktracking();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        static void TestMCP()
        {
            //TestMCP(0);
        }

        static int TestMCP(int seed, int size)
        {
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = size;
            riddle.Y = size;
            riddle.Seed = seed;
            riddle.Initialize();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;
                if (solutionCount == 1)
                {
                    MapVisualizer image = new MapVisualizer(xd.Select(v => v as Variable).ToList(),
                        riddle.Connections, riddle.X, riddle.Y);
                    image.Save("example.bmp");
                }
            };
            riddle.RunBacktracking();
            //Console.Write("BT: " + riddle.Counter);
            //Console.WriteLine("Solutions: " + solutionCount);
            return riddle.Counter;
        }

        static void TestEinsteinFC()
        {
            int solutionCount = 0;
            EinsteinRiddleProblem riddle = new EinsteinRiddleProblem();
            riddle.Initialize();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;

                EinsteinRiddleProblem.DisplaySolution(xd);
            };
            riddle.RunForwardChecking();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        static void TestMCPFC()
        {
            //TestMCPFC(0);
        }

        static int TestMCPFC(int seed, int size)
        {
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = size;
            riddle.Y = size;
            riddle.Seed = seed;
            riddle.Initialize();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;
                if (solutionCount == 1)
                {
                    MapVisualizer image = new MapVisualizer(xd.Select(v => v as Variable).ToList(),
                        riddle.Connections, riddle.X, riddle.Y);
                    image.Save("example.bmp");
                }
            };
            riddle.RunForwardChecking();
            sum += riddle.Counter;
            //Console.WriteLine(" FC: " + riddle.Counter);
            return riddle.Counter;
        }
    }
}
