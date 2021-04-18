using System;
using CSP.EinsteinRiddle;
using CSP.MapColouring;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestEinstein();
            TestMCP();
        }

        static void TestEinstein()
        {
            int solutionCount = 0;
            EinsteinRiddleProblem riddle = new EinsteinRiddleProblem();
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
            MapColouringNaPale riddle = new MapColouringNaPale();
            riddle.X = 3;
            riddle.Y = 3;
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;
            };
            riddle.RunBacktracking();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        // TODO WIZUALIZACJA MCP
        // Generowanie kozackiej bitmapki

        // TODO NAPRAWIĆ GENEROWANIE
        // bug: niepoprawna detekcja kolizji dla pionowych linii?
    }
}
