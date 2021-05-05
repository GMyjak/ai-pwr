using System;
using System.Collections.Generic;
using System.Text;

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
                    MinMax mm = new MinMax(state, Player.B);
                    int move = mm.Move();
                    state.Move(Player.B, move);

                    Console.WriteLine("BOT moved to " + move + "\n");
                }
            }

            Console.WriteLine("GAME OVER\n");
            Console.WriteLine(state);
            Console.WriteLine("\nWinner: " + state.GetWinningPlayer());
        }

        public void GetPlayerMove()
        {
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

                state.Move(state.CurrentPlayer, parsed);
                Console.WriteLine("Moved successfully\n");
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }
}
