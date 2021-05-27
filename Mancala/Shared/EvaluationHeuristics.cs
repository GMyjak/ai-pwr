using System.Linq;

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
                score += game.PlayerAWell * wellWeight;
                score += game.PlayerAHoles.Sum(h => h) * holeSumWeight;

                score -= game.PlayerBWell * wellWeight;
                score -= game.PlayerBHoles.Sum(h => h) * enemyHoleSumWeight;
            }
            else
            {
                score += game.PlayerBWell * wellWeight;
                score += game.PlayerBHoles.Sum(h => h) * holeSumWeight;

                score -= game.PlayerAWell * wellWeight;
                score -= game.PlayerAHoles.Sum(h => h) * enemyHoleSumWeight;
            }

            return score;
        }
    }
}
