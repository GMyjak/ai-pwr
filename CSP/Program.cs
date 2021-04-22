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
            //TestEinstein();
            for (int i = 0; i < 10; i++)
            {
                TestMCP();
            }
            //TestEinsteinFC();
            for (int i = 0; i < 10; i++)
            {
                TestMCPFC();
            }
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
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = 7;
            riddle.Y = 7;
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
            int solutionCount = 0;
            MapColoringProblem riddle = new MapColoringProblem();
            riddle.X = 7;
            riddle.Y = 7;
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

        // TODO Naprawić te testy bo nie mogę na nie patrzeć xD
        // TODO Seedy i kompnentyzacja kodu ;)
    }
}
