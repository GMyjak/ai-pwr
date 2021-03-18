using System;
using System.Collections.Generic;
using System.Linq;

namespace Genetics
{
    class GeneticAlgorithm
    {
        public Problem Problem { get; set; }
        public int PopulationSize { get; set; }
        public int GenerationCount { get; set; }
        public float CrossProbability { get; set; }
        public float MutationProbability { get; set; }
        public int Seed { get; set; } = 3333;

        public Func<Problem, int, List<Individual>> InitFactoryMethod { get; set; }
        public Action<int, List<Individual>> OnIterationFinished { get; set; } = (_,__) => { };

        private int generationIndex = 0;
        private List<Individual> population;

        public void Run()
        {
            Random rng = new Random(Seed);

            generationIndex = 0;
            population = InitFactoryMethod(Problem, PopulationSize);
            population.ForEach(i => i.Score = i.Evaluate());
            float minimalScore = population.Min(i => i.Score.Value);
            population.ForEach(i => i.AdaptToPopulation(minimalScore));
            OnIterationFinished(generationIndex, population);

            while (generationIndex < GenerationCount)
            {
                List<Individual> newPopulation = new List<Individual>();

                while (newPopulation.Count < PopulationSize)
                {
                    Individual individual1 = Individual.Select(population, SelectionType.Tournament);
                    Individual individual2 = Individual.Select(population, SelectionType.Tournament);
                    Individual newIndividual = rng.NextDouble() < CrossProbability ? Individual.Cross(individual1, individual2) : individual1.Copy();

                    if (rng.NextDouble() < MutationProbability)
                    {
                        newIndividual.Mutate();
                    }

                    newIndividual.Score = newIndividual.Evaluate();
                    newPopulation.Add(newIndividual);
                }

                minimalScore = newPopulation.Min(i => i.Score.Value);
                newPopulation.ForEach(i => i.AdaptToPopulation(minimalScore));

                population = newPopulation;
                generationIndex++;
                OnIterationFinished(generationIndex, population);
            }
        }
    }
}
