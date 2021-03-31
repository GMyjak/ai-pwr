using Genetics.StructureDefinitions;

namespace Genetics
{
    abstract class Config
    {
        // Solution penalties
        public static float crossPenalty;
        public static float pathOutsideBoardPenalty;
        public static float pathOutsideBoardPenaltyWeight;
        public static float segmentNumberWeight;
        public static float pathLengthWeight;

        // GA
        public static int tournamentSize;
        public static int populationSize;
        public static int generationCount;
        public static float crossProbability;
        public static float mutationProbability;
        public static SelectionType selectionType;

        public static void Reset()
        {
            crossPenalty = 10f;
            pathOutsideBoardPenalty = 50f;
            pathOutsideBoardPenaltyWeight = 2f;
            segmentNumberWeight = 0.25f;
            pathLengthWeight = 0.05f;
            tournamentSize = 7;
            populationSize = 300;
            generationCount = 1000;
            crossProbability = 0.5f;
            mutationProbability = 0.35f;
            SelectionType selectionType = SelectionType.Tournament;
        }
    }
}
