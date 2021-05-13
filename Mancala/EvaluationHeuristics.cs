using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mancala
{
    class EvaluationHeuristics
    {
        private static float keepTurnBonus = 3;
        private static float wellWeight = 2;
        private static float holeSumWeight = 0.5f;
        private static float enemyHoleSumWeight = 0.3f;

        public static float TestEvaluation(Game game, Player player)
        {
            float score = 0;

            if (game.CurrentPlayer == player && !game.PassFlag)
            {
                score += keepTurnBonus;
            }
            else if (game.CurrentPlayer != player && game.PassFlag)
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
