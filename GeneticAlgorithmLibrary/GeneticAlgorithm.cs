using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmLibrary
{
    public class GeneticAlgorithm<T>
    {
        public static Random RANDOM = new Random();

        public T[] population;
        public double[] fitness;
        public double mutationRate;
        public double replicationRate;
        public int generation;
        public Func<T> InitializeEntity;
        public Func<T, double> Fitness;
        public Func<T, T, T> Crossover;
        public Func<T, double, T> Mutate;

        public bool useCrossover = true;
        public bool useMutation = true;
        public bool useElitism = true;

        public T Best { get; private set; }
        public double BestFitness { get; private set; }

        public GeneticAlgorithm(int populationSize, double mutationRate, double replicationRate,
            Func<T> initializeFunction, Func<T, double> fitnessFunction,
            Func<T, T, T> crossoverFunction, Func<T, double, T> mutateFunction)
        {
            InitializeEntity = initializeFunction ?? NoInitialize;
            Fitness = fitnessFunction ?? NoFitness;
            Crossover = crossoverFunction ?? NoCrossOver;
            Mutate = mutateFunction ?? NoMutate;

            population = new T[populationSize];
            for (var i = 0; i < populationSize; i++)
                population[i] = InitializeEntity.Invoke();

            CalculateFitness();
            this.mutationRate = mutationRate;
            this.replicationRate = replicationRate;
        }

        public void SetPopulation(IEnumerable<T> newPopulation)
        {
            population = newPopulation.ToArray();
            CalculateFitness();
        }

        public void CalculateFitness()
        {
            fitness = new double[population.Length];
            for (var i = 0; i < population.Length; i++)
                fitness[i] = Fitness.Invoke(population[i]);

            BestFitness = double.NegativeInfinity;
            for (var i = 0; i < population.Length; i++)
                if (BestFitness < fitness[i])
                {
                    BestFitness = fitness[i];
                    Best = population[i];
                }
        }

        public void Evolve()
        {
            generation++;
            var newPopulation = new T[population.Length];
            for (var i = 0; i < population.Length; i++)
            {
                // Copy Previous Population
                newPopulation[i] = population[i];

                // Replication
                var shouldReplicate = RandomDouble01();
                if (shouldReplicate <= replicationRate) continue;

                // Crossover
                if (useCrossover)
                {
                    var parentA = PickParent();
                    var parentB = PickParent();
                    newPopulation[i] = Crossover.Invoke(parentA, parentB);
                }

                // Mutate
                if (useMutation)
                    newPopulation[i] = Mutate.Invoke(newPopulation[i], mutationRate);
            }

            // Elitism [Copy best of population to next generation]
            if (useElitism)
                newPopulation[0] = Best;

            population = newPopulation;
            CalculateFitness();
        }

        private T PickParent()
        {
            var sum = 0.0;
            for (var i = 0; i < fitness.Length; i++)
                sum += fitness[i];

            var selection = RandomDouble01();
            var accumulation = 0.0;
            for (var i = 0; i < fitness.Length; i++)
            {
                accumulation += fitness[i];
                if (selection <= accumulation / sum)
                    return population[i];
            }

            return population[population.Length - 1];
        }

        private T NoInitialize() => default(T);
        private double NoFitness(T entity) => 0;
        private T NoCrossOver(T parentA, T parentB) => parentA;
        private T NoMutate(T entity, double mutationRate) => entity;

        private static double RandomDouble01() => RANDOM.NextDouble();
    }
}