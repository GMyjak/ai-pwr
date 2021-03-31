using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.StructureDefinitions;

namespace Genetics
{
    class GeneticAlgorithm
    {
        public Problem Problem { get; set; }

        public Func<Problem, int, List<Individual>> InitFactoryMethod { get; set; }
        public Action<int, List<Individual>> OnIterationFinished { get; set; } = (_,__) => { };

        private int generationIndex = 0;
        private List<Individual> population;

        public Individual Run()
        {
            Random rng = UniversalRandom.Rng;

            generationIndex = 0;
            population = InitFactoryMethod(Problem, Config.populationSize);
            population.ForEach(i => i.Penalty = i.Evaluate());
            BulkEvaluate();

            OnIterationFinished(generationIndex, population);

            while (generationIndex < Config.generationCount)
            {
                List<Individual> newPopulation = new List<Individual>();

                while (newPopulation.Count < Config.populationSize)
                {
                    Individual individual1 = Individual.Select(population, Config.selectionType);
                    Individual individual2 = Individual.Select(population, Config.selectionType);
                    Individual newIndividual = rng.NextDouble() < Config.crossProbability ? Individual.Cross(individual1, individual2) : individual1.Copy();

                    if (rng.NextDouble() < Config.mutationProbability)
                    {
                        newIndividual.Mutate();
                    }

                    newIndividual.Penalty = newIndividual.Evaluate();
                    newPopulation.Add(newIndividual);
                }

                BulkEvaluate();

                population = newPopulation;
                generationIndex++;
                OnIterationFinished(generationIndex, population);
            }

            float minPenalty = population.Min(i => i.Penalty);
            Individual best = population.First(i => i.Penalty == minPenalty);
            return best;
        }

        private void BulkEvaluate()
        {
            float minimalPenalty = population.Min(i => i.Penalty);
            population.ForEach(i => i.AdaptToPopulation(minimalPenalty));
        }
    }
}
