using System;

namespace Mancala
{
    public enum PlayerType
    {
        Human,
        Computer
    }

    public class ConsoleUI
    {
        private Game state = new Game();
        
        public PlayerType p1 = PlayerType.Human;
        public PlayerType p2 = PlayerType.Computer;
        public bool firstAiMoveRandom = false;
        public Algo algorithm = Algo.AlphaBeta;
        public Func<Game, Player, float> playerAHeuristic = EvaluationHeuristics.TestEvaluation;
        public Func<Game, Player, float> playerBHeuristic = EvaluationHeuristics.TestEvaluation;
        public bool verbose = false;
        public Action<Player?> OnPlayerWon = (_) => { };

        public void Run()
        {
            bool loopFlag = true;
            state.OnGameOver += () => { loopFlag = false; };

            if (firstAiMoveRandom)
            {
                state.MakeRandomMove();
                state.MakeRandomMove();
            }

            while (loopFlag)
            {
                if (verbose)
                {
                    Console.WriteLine(state + "\n");
                }
                
                if (state.CurrentPlayer == Player.A)
                {
                    if (p1 == PlayerType.Human)
                    {
                        GetPlayerMove();
                    }
                    else
                    {
                        GetAiMove();
                    }
                }
                else
                {
                    if (p2 == PlayerType.Human)
                    {
                        GetPlayerMove();
                    }
                    else
                    {
                        GetAiMove();
                    }
                }
            }

            if (verbose)
            {
                Console.WriteLine("GAME OVER\n");
                Console.WriteLine(state);
            }
            Console.WriteLine("\nWinner: " + state.GetWinningPlayer());
            OnPlayerWon?.Invoke(state.GetWinningPlayer());
        }

        private void GetAiMove()
        {
            MancalaAi mm = new MancalaAi(state, state.CurrentPlayer, algorithm);
            if (state.CurrentPlayer == Player.A)
            {
                mm.EvaluationFunction = playerAHeuristic;
            }
            else
            {
                mm.EvaluationFunction = playerBHeuristic;
            }
            var move = mm.Move();
            if (move.Pass && verbose)
            {
                Console.WriteLine($"BOT {state.CurrentPlayer} forced to skip\n");
            }
            else if (verbose)
            {
                Console.WriteLine($"BOT {state.CurrentPlayer} moved to {move.MoveIndex}\n");
            }
            state.Move(state.CurrentPlayer, move);
        }

        private void GetPlayerMove()
        {
            var moves = state.GetAvailableMoves();
            if (moves.Count == 1 && moves[0].Pass)
            {
                Console.WriteLine($"Player {state.CurrentPlayer} forced to skip");
                state.Move(state.CurrentPlayer, moves[0]);
                return;
            }

            Console.WriteLine(state + "\n"); 
            Console.WriteLine("Player " + state.CurrentPlayer + " move: ");
            
            
            string input = Console.ReadLine();
            if (int.TryParse(input, out var parsed))
            {
                if (!state.IsValidMove(parsed))
                {
                    Console.WriteLine("Invalid move\n");
                    return;
                }

                state.Move(state.CurrentPlayer, Move.NormalMove(parsed));
                Console.WriteLine("Moved successfully\n");
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }
}
