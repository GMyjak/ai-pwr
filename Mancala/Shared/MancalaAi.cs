using System;
using System.Collections.Generic;

namespace Mancala
{
    public enum Algo
    {
        MinMax,
        AlphaBeta
    }

    public class MancalaAi
    {
        public int Depth { get; set; } = 8;

        private Game game;
        private Player player;
        private Algo algorithm;
        private Func<Game, Player, float> EvaluationFunction = EvaluationHeuristics.TestEvaluation;

        public MancalaAi(Game game, Player player, Algo algorithm)
        {
            this.game = game;
            this.player = player;
            this.algorithm = algorithm;
        }

        public Move Move()
        {
            if (game.PassFlag)
            {
                return Mancala.Move.PassingMove();
            }

            if (algorithm == Algo.MinMax)
            {
                MinMaxRec(Depth, game, player, out int index);
                return Mancala.Move.NormalMove(index);
            }
            else
            {
                AlphaBetaRec(Depth, game, player, -1000000, 1000000, out int index);
                return Mancala.Move.NormalMove(index);
            }
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

        private float AlphaBetaRec(int depth, Game nextState, Player p, float alpha, float beta, out int bestIndex)
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
                        float eval = AlphaBetaRec(depth - 1, child, child.CurrentPlayer, alpha, beta, out _);
                        if (eval > value)
                        {
                            bestIndex = i;
                            value = eval;
                        }

                        alpha = Math.Max(alpha, value);
                        if (alpha >= beta)
                        {
                            break;
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
                        float eval = AlphaBetaRec(depth - 1, child, child.CurrentPlayer, alpha, beta, out _);
                        if (eval < value)
                        {
                            bestIndex = i;
                            value = eval;
                        }

                        beta = Math.Min(beta, value);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return value;
            }
        }
    }
}
