using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mancala
{
    public class MinMax
    {
        public int Depth { get; set; } = 10;

        private Game game;
        private Player player;
        private static float keepTurnBonus = 3;
        private static float wellWeight = 2;
        private static float holeSumWeight = 0.5f;
        private static float enemyHoleSumWeight = 0.3f;

        public MinMax(Game game, Player player)
        {
            this.game = game;
            this.player = player;
        }

        public int Move()
        {
            MinMaxRec(Depth, game, player, out int index);
            return index;
        }

        // Return best score, out best index to achieve that score
        public float MinMaxRec(int depth, Game nextState, Player p, out int bestIndex)
        {
            bestIndex = -1;
            if (depth == 0 || nextState.IsGameOver())
            {
                return EvaluatePosition(nextState, player);
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
                        float eval = EvaluatePosition(child, player);
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
                        float eval = EvaluatePosition(child, player);
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

        //public int Max(int depth, Game state, Player p)
        //{
        //    List<Game> states = new List<Game>();
        //    for (int i = 0; i < state.GameSize; i++)
        //    {
        //        if (state.IsValidMove(i))
        //        {
        //            states.Add(state.TryMove(state.CurrentPlayer, i, out _));
        //        }
        //        else
        //        {
        //            states.Add(null);
        //        }
        //    }

        //    if (depth == 0)
        //    {
        //        List<int?> evaluations = new List<int?>();
        //        foreach (var stateVar in states)
        //        {
        //            if (stateVar == null)
        //            {
        //                evaluations.Add(null);
        //            }
        //            else
        //            {
        //                evaluations.Add(EvaluatePosition(stateVar, player));
        //            }
        //        }

        //        int bestEval = evaluations.Max(ev => ev == null ? -100000 : ev.Value);
        //        return evaluations.IndexOf(bestEval);
        //    }
        //    else
        //    {
        //        // TOO DOO ECKSS DEE ;)
        //    }
        //}

        public static float EvaluatePosition(Game game, Player player)
        {
            float score = 0;

            if (game.CurrentPlayer == player)
            {
                score += keepTurnBonus;
            }

            if (player == Player.A)
            {
                score += game.playerAWell * wellWeight;
                score += game.playerAHoles.Sum(h => h) * holeSumWeight;

                score -= game.playerBWell * wellWeight;
                score -= game.playerBHoles.Sum(h => h) * enemyHoleSumWeight;
            }
            else
            {
                score += game.playerBWell * wellWeight;
                score += game.playerBHoles.Sum(h => h) * holeSumWeight;

                score -= game.playerAWell * wellWeight;
                score -= game.playerAHoles.Sum(h => h) * enemyHoleSumWeight;
            }

            return score;
        }
    }
}
