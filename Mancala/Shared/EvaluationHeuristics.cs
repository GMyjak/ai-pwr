using System;
using System.Linq;

namespace Mancala
{
    class EvaluationHeuristics
    {
        private static float keepTurnBonus = 1;
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

        // Since stones in normal holes will count towards final score, 
        // it is important to preserve them
        public static float CompareScores(Game game, Player player)
        {
            float score = 0;

            if (player == Player.A)
            {
                score += game.PlayerAWell;
                score += game.PlayerAHoles.Sum(h => h);

                score -= game.PlayerBWell;
                score -= game.PlayerBHoles.Sum(h => h);
            }
            else
            {
                score += game.PlayerBWell;
                score += game.PlayerBHoles.Sum(h => h);

                score -= game.PlayerAWell;
                score -= game.PlayerAHoles.Sum(h => h);
            }

            return score;
        }

        // It is worth to keep large stack close to your well, because:
        // a - it is easier to strike your opponent's stones
        // b - it is harder for opponent to strike player

        // Cannot be 0
        private static float firstHoleOffset = 4f;

        public static float CompareScoresAdaptiveHoles(Game game, Player player)
        {
            float score = 0;

            if (player == Player.A)
            {
                score += game.PlayerAWell;
                for (int i = 0; i < 6; i++)
                {
                    float l = i + firstHoleOffset;
                    float m = 6 + firstHoleOffset;
                    score += game.PlayerAHoles[i] * (l / m);
                }

                score -= game.PlayerBWell;
                score -= game.PlayerBHoles.Sum(h => h);
            }
            else
            {
                score += game.PlayerBWell;
                for (int i = 0; i < 6; i++)
                {
                    float l = i + firstHoleOffset;
                    float m = 6 + firstHoleOffset;
                    score += game.PlayerBHoles[i] * (l / m);
                }

                score -= game.PlayerAWell;
                score -= game.PlayerAHoles.Sum(h => h);
            }

            return score;
        }

        // This should be pretty bad, but why no try
        // Stones in well cannot be struck
        public static float PrioritizeWell(Game game, Player player)
        {
            float score = game.PlayerAWell - game.PlayerBWell;
            return player == Player.A ? score : -score;
        }

        // Whatever man xD
        public static float Bullshit(Game game, Player player)
        {
            Random rng = new Random();
            return rng.Next();
        }
    }
}
