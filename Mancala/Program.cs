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

            //CompareAlgorithms(10);

            //SimpleEvETest(Algo.AlphaBeta);

            int playerACounter = 0;
            int playerBCounter = 0;

            for (int i = 0; i < 20; i++)
            {
                SimpleEvETest(Algo.AlphaBeta, 
                    EvaluationHeuristics.CompareScoresAdaptiveHoles,
                    EvaluationHeuristics.TestEvaluation,  
                    playerWon: (p) =>
                {
                    if (p.HasValue && p.Value == Player.A)
                    {
                        playerACounter++;
                    }
                    else if (p.HasValue && p.Value == Player.B)
                    {
                        playerBCounter++;
                    }
                });
            }

            Console.WriteLine($"A: {playerACounter}, B: {playerBCounter}");
        }

        static void SimpleEvETest(Algo algo, Func<Game, Player, float> heuristicA = null,
            Func<Game, Player, float> heuristicB = null, Action<Player?> playerWon = null)
        {
            ConsoleUI ui = new ConsoleUI()
            {
                p1 = PlayerType.Computer,
                p2 = PlayerType.Computer,
                firstAiMoveRandom = true,
                algorithm = algo,
            };
            if (heuristicA != null)
            {
                ui.playerAHeuristic = heuristicA;
            }
            if (heuristicB != null)
            {
                ui.playerBHeuristic = heuristicB;
            }

            if (playerWon != null)
            {
                ui.OnPlayerWon += playerWon;
            }

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
