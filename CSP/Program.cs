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
            EinsteinRiddleNaPale riddle = new EinsteinRiddleNaPale();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;

                EinsteinRiddleNaPale.DisplaySolution(xd);
            };
            riddle.RunAlgo();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        static void TestMCP()
        {
            int solutionCount = 0;
            MapColouringNaPale riddle = new MapColouringNaPale();
            riddle.GenerateInstance(5, 5);
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;
            };
            riddle.RunAlgo();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        // TODO WIZUALIZACJA MCP
        // TODO NAPRAWIĆ GENEROWANIE
    }
}
