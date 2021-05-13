using System;

namespace Mancala
{
    public class ConsoleUI
    {
        private Game state = new Game();

        public void RunPVP()
        {
            bool loopFlag = true;
            state.OnGameOver += () => { loopFlag = false; };
            while (loopFlag)
            {
                GetPlayerMove();
            }

            Console.WriteLine("GAME OVER");
        }

        public void RunPVE()
        {
            bool loopFlag = true;
            state.OnGameOver += () => { loopFlag = false; };
            while (loopFlag)
            {
                if (state.CurrentPlayer == Player.A)
                {
                    GetPlayerMove();
                }
                else
                {
                    GetAiMove();
                }
            }

            Console.WriteLine("GAME OVER\n");
            Console.WriteLine(state);
            Console.WriteLine("\nWinner: " + state.GetWinningPlayer());
        }

        private void GetAiMove()
        {
            MinMax mm = new MinMax(state, Player.B);
            var move = mm.Move();
            state.Move(Player.B, move);

            if (move.Pass)
            {
                Console.WriteLine("BOT forced to skip\n");
            }
            else
            {
                Console.WriteLine("BOT moved to " + move.MoveIndex + "\n");
            }
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
