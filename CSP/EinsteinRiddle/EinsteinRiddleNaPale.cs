using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSP.EinsteinRiddle
{
    class EinsteinRiddleNaPale
    {
        // Ziomki
        // 1 - Norweg
        // 2 - Anglik
        // 3 - Duńczyk
        // 4 - Niemiec
        // 5 - Szwed

        // Kolory
        // 1 - czerwony
        // 2 - zielony
        // 3 - biały
        // 4 - żółty
        // 5 - niebieski

        // Fajki
        // 1 - light
        // 2 - bez filtra
        // 3 - mentolowe
        // 4 - cygara
        // 5 - fajka

        // Napoje
        // 1 - herbata
        // 2 - mleko
        // 3 - woda
        // 4 - piwo
        // 5 - kawa

        // Zwierzaki
        // 1 - kot
        // 2 - ptak
        // 3 - pies
        // 4 - koń
        // 5 - smok






        private List<Func<bool>> constraints = new List<Func<bool>>()
        {
            () => true,

        };
        private List<Variable> variables;


        public List<Permutation> RunAlgo()
        {
            variables = new List<Variable>();

            for (int i = 0; i < 5; i++)
            {
                variables.Add(new Variable()
                {
                    Current = null,
                    Domain = Permutation.GetAllPermutations()
                });
            }

            return null;
        }
    }

    class Variable
    {
        public Permutation Current { get; set; }
        public List<Permutation> Domain { get; set; }
    }

    class Permutation
    {
        public int[] Values { get; set; }

        public static List<Permutation> GetAllPermutations()
        {
            var perms = GetAllPermutations(new List<int>() {1, 2, 3, 4, 5}, 5);
            var result = new List<Permutation>();

            foreach (var perm in perms)
            {
                result.Add(new Permutation()
                {
                    Values = perm.ToArray()
                });
            }

            return result;
        }

        public static IEnumerable<IEnumerable<int>> GetAllPermutations(IEnumerable<int> initial, int length)
        {
            
            if (length == 1) return initial.Select(t => new int[] { t });

            return GetAllPermutations(initial, length - 1)
                .SelectMany(t => initial.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new int[] { t2 }));
        }
    }
}
