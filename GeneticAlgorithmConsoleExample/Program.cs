using System;
using GeneticAlgorithmLibrary;

namespace GeneticAlgorithmConsoleExample
{
    internal class Program
    {
        public static Random Random = new Random();

        private static string target = "To be or not to be.";
        private static int minSize = 1;
        private static int maxSize = 32;

        public static void Main(string[] args)
        {
            var geneticAlgorithm = new GeneticAlgorithm<string>
            (
                populationSize: 50,
                mutationRate: 0.01,
                replicationRate: 0.01,
                initializeFunction: GetRandomString,
                fitnessFunction: CalculateFitness,
                crossoverFunction: Crossover,
                mutateFunction: Mutate
            );

            for (var i = 0; i < 1000000; i++)
            {
                Console.WriteLine(
                    $"generation: {geneticAlgorithm.generation} fitness: {geneticAlgorithm.BestFitness} best: {geneticAlgorithm.Best}");

                if (geneticAlgorithm.Best == target)
                    break;

                geneticAlgorithm.Evolve();
            }
        }

        private static string GetRandomString()
        {
            return RandomString(RandomInt(minSize, maxSize));
        }

        private static double CalculateFitness(string entity)
        {
            var range = maxSize - minSize;
            var lengthDistance = Math.Abs(target.Length - entity.Length);
            var lengthFitness = range - lengthDistance;

            var matchingLetters = 0;
            for (var i = 0; i < target.Length && i < entity.Length; i++)
                if (target[i] == entity[i])
                    matchingLetters++;
            var matchingFitness = matchingLetters;

            return lengthFitness + matchingFitness * 2;
        }

        private static string Crossover(string parentA, string parentB)
        {
            if (parentA.Length > parentB.Length)
            {
                var temp = parentB;
                parentB = parentA;
                parentA = temp;
            }

            var cutoff = RandomInt(0, parentA.Length);
            var length = RandomInt(parentA.Length, parentB.Length + 1);
            var child = "";
            for (var i = 0; i < length; i++)
            {
                if (i < cutoff)
                    child += parentA[i];
                else
                    child += parentB[i];
            }

            return child;
        }

        private static string Mutate(string str, double mutationRate)
        {
            var mutant = "";
            var selection = RandomFloat01();
            var length = str.Length;
            if (selection < mutationRate)
                length += RandomBool() ? 1 : -1;

            if (length > str.Length)
                str += RandomChar();

            for (var i = 0; i < length; i++)
            {
                selection = RandomFloat01();
                if (selection < mutationRate)
                    mutant += RandomChar();
                else
                    mutant += str[i];
            }

            return mutant;
        }

        public static int RandomInt(int minValue, int maxValue) => Random.Next(minValue, maxValue);

        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789. ";
        public static char RandomChar() => chars[Random.Next(chars.Length)];

        public static string RandomString(int length)
        {
            var str = "";
            for (var i = 0; i < length; i++)
                str += RandomChar();
            return str;
        }

        public static bool RandomBool() => Random.Next(2) == 1;

        public static float RandomFloat01() => (float) Random.NextDouble();
    }
}