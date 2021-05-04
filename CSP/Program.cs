using System;
using System.Linq;
using CSP.EinsteinRiddle;
using CSP.MapColouring;
using Variable = CSP.MapColouring.Variable;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rng = new Random();
            int seed = rng.Next();
            //TestEinstein();
            TestMCP(seed);
            //TestEinsteinFC();
            TestMCPFC(seed);
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
            TestMCP(0);
        }

        static void TestMCP(int seed)
        {
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = 7;
            riddle.Y = 7;
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
            Console.WriteLine("Solutions: " + solutionCount);
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
            TestMCPFC(0);
        }

        static void TestMCPFC(int seed)
        {
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = 7;
            riddle.Y = 7;
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
            Console.WriteLine("Solutions: " + solutionCount);
        }

        // TODO NAPRAWIĆ GENEROWANIE
        // bug: niepoprawna detekcja kolizji dla pionowych linii? definitywnie coś jest rozjebane ale na razie nie wiem co xd
    }
}
