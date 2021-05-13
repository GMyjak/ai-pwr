using System;
using System.Collections.Generic;

namespace Mancala
{
    public class MinMax
    {
        public int Depth { get; set; } = 8;

        private Game game;
        private Player player;
        private Func<Game, Player, float> EvaluationFunction = EvaluationHeuristics.TestEvaluation;

        public MinMax(Game game, Player player)
        {
            this.game = game;
            this.player = player;
        }

        public Move Move()
        {
            if (game.PassFlag)
            {
                return Mancala.Move.PassingMove();
            }

            MinMaxRec(Depth, game, player, out int index);
            return Mancala.Move.NormalMove(index);
        }

        // Return best score, out best index to achieve that score
        private float MinMaxRec(int depth, Game nextState, Player p, out int bestIndex)
        {
            bestIndex = -1;
            if (depth == 0 || nextState.IsGameOver())
            {
                return EvaluationFunction(nextState, player);
            }

            List<Game> children = new List<Game>();
            for (int i = 0; i < nextState.GameSize; i++)
            {
                if (nextState.IsValidMove(i))
                {
                    children.Add(nextState.TryMove(nextState.CurrentPlayer, i, out _));
                }
                else
                {
                    children.Add(null);
                }
            }

            // if maximizing player
            if (nextState.CurrentPlayer == player)
            {
                float value = -1000000;
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child != null)
                    {
                        float eval = MinMaxRec(depth - 1, child, child.CurrentPlayer, out _);
                        if (eval > value)
                        {
                            bestIndex = i;
                            value = eval;
                        }
                    }
                }

                return value;
            }
            else
            {
                float value = 1000000;
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child != null)
                    {
                        float eval = MinMaxRec(depth - 1, child, child.CurrentPlayer, out _);
                        if (eval < value)
                        {
                            bestIndex = i;
                            value = eval;
                        }
                    }
                }

                return value;
            }
        }
    }
}
