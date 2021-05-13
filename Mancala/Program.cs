using System;
using System.Diagnostics;

namespace Mancala
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConsoleUI ui = new ConsoleUI()
            //{
            //    p1 = PlayerType.Computer,
            //    p2 = PlayerType.Computer,
            //    firstAiMoveRandom = true,
            //    //algorithm = Algo.MinMax,
            //    algorithm = Algo.AlphaBeta,
            //};

            //ui.Run();

            CompareAlgorithms(10);
        }

        static void SimpleEvETest(Algo algo)
        {
            ConsoleUI ui = new ConsoleUI()
            {
                p1 = PlayerType.Computer,
                p2 = PlayerType.Computer,
                firstAiMoveRandom = true,
                algorithm = algo,
            };

            ui.Run();
        }

        static void CompareAlgorithms(int testSize)
        {
            Stopwatch sw = new Stopwatch();
            long alphaBetaTimer = 0, minMaxTimer = 0;

            sw.Start();
            for (int i = 0; i < testSize; i++)
            {
                SimpleEvETest(Algo.AlphaBeta);
            }
            sw.Stop();
            alphaBetaTimer = sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();
            for (int i = 0; i < testSize; i++)
            {
                SimpleEvETest(Algo.MinMax);
            }
            sw.Stop();
            minMaxTimer = sw.ElapsedMilliseconds;

            Console.WriteLine($"AlphaBeta x {testSize}: {alphaBetaTimer}ms");
            Console.WriteLine($"MinMax x {testSize}: {minMaxTimer}ms");
        }
    }
}
