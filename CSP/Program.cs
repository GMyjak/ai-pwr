using System;
using CSP.EinsteinRiddle;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
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
    }
}
