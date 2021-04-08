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

        private List<Func<bool>> constraints;
        private List<Variable> variables;

        public List<Permutation> RunAlgo()
        {
            DefineVariables();
            DefineConstraints();

            FindSolution(0);

            return variables.Select(v => v.Current).ToList();
        }

        bool FindSolution(int variableIndex)
        {
            if (variableIndex == variables.Count)
            {
                return true;
            }

            foreach (var permutation in variables[variableIndex].Domain)
            {
                variables[variableIndex].Current = permutation;
                if (constraints.All(con => con.Invoke()))
                {
                    return FindSolution(variableIndex + 1);
                }
                else
                {
                    variables[variableIndex].Current = null;
                }
            }

            return false;
        }

        public void DefineVariables()
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
        }

        public void DefineConstraints()
        {
            constraints = new List<Func<bool>>();

            // R1
            constraints.Add(() =>
            {
                return variables[0].Current == null || variables[0].Current.Values[0] == 1;
            });

            // R2
            constraints.Add(() =>
            {
                return variables[0].Current == null || variables[1].Current == null ||
                       Array.IndexOf(variables[0].Current.Values, 2) == Array.IndexOf(variables[1].Current.Values, 1);
            });

            // R3
            constraints.Add(() =>
            {
                return variables[1].Current == null || Array.IndexOf(variables[1].Current.Values, 2) ==
                    Array.IndexOf(variables[1].Current.Values, 3) - 1;
            });

            // R4
            constraints.Add(() =>
            {
                return variables[0].Current == null || variables[3].Current == null ||
                       Array.IndexOf(variables[0].Current.Values, 3) == Array.IndexOf(variables[3].Current.Values, 1);
            });

            // R5
            constraints.Add(() =>
            {
                return variables[2].Current == null || variables[4].Current == null || 
                       Math.Abs(Array.IndexOf(variables[2].Current.Values, 1) - Array.IndexOf(variables[4].Current.Values, 1)) == 1;
            });
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
