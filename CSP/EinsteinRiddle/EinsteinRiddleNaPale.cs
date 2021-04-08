using System;
using System.Collections.Generic;
using System.Linq;

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

        public Action<List<Permutation>> OnSolutionFound { get; set; } = (_) => { };

        public void RunAlgo()
        {
            DefineVariables();
            DefineConstraints();

            FindSolution(0);
        }

        private void FindSolution(int variableIndex)
        {
            if (variableIndex == variables.Count)
            {
                OnSolutionFound?.Invoke(variables.Select(v => v.Current).ToList());
                return;
            }

            foreach (var permutation in variables[variableIndex].Domain)
            {
                variables[variableIndex].Current = permutation;
                if (constraints.All(con => con.Invoke()))
                {
                    FindSolution(variableIndex + 1);
                }
                else
                {
                    variables[variableIndex].Current = null;
                }
            }
        }

        private void DefineVariables()
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

        private void DefineConstraints()
        {
            constraints = new List<Func<bool>>();

            // R1
            constraints.Add(GenerateUnary(0, 1, 0));

            // R2
            constraints.Add(GenerateBinary(0, 2, 1, 1));

            // R3
            constraints.Add(() =>
            {
                return variables[1].Current == null || Array.IndexOf(variables[1].Current.Values, 2) ==
                    Array.IndexOf(variables[1].Current.Values, 3) - 1;
            });

            // R4
            constraints.Add(GenerateBinary(0, 3, 3, 1));

            // R5
            constraints.Add(GenerateBinaryNeighbour(2, 1, 4, 1));

            // R6
            constraints.Add(GenerateBinary(1, 4, 2, 4));

            // R7
            constraints.Add(GenerateBinary(0, 4, 2, 5));

            // R8
            constraints.Add(GenerateUnary(3, 2, 2));

            // R9
            constraints.Add(GenerateBinaryNeighbour(2, 1, 3, 3));

            // R10
            constraints.Add(GenerateBinary(2, 2, 4, 2));

            // R11
            constraints.Add(GenerateBinary(0, 5, 4, 3));

            // R12
            constraints.Add(GenerateBinaryNeighbour(0, 1, 1, 5));

            // R13
            constraints.Add(GenerateBinaryNeighbour(4, 4, 1, 4));

            // R14
            constraints.Add(GenerateBinary(2, 3, 3, 4));

            // R15
            constraints.Add(GenerateBinary(1, 2, 3, 5));
        }

        private Func<bool> GenerateUnary(int varId, int value, int index)
        {
            return () => variables[varId].Current == null || variables[varId].Current.Values[index] == value;
        }

        private Func<bool> GenerateBinary(int var1Id, int value1Id, int var2Id, int value2Id)
        {
            return () => variables[var1Id].Current == null || variables[var2Id].Current == null ||
                         Array.IndexOf(variables[var1Id].Current.Values, value1Id) == Array.IndexOf(variables[var2Id].Current.Values, value2Id);
        }

        private Func<bool> GenerateBinaryNeighbour(int var1Id, int value1Id, int var2Id, int value2Id)
        {
            return () => variables[var1Id].Current == null || variables[var2Id].Current == null ||
                   Math.Abs(Array.IndexOf(variables[var1Id].Current.Values, value1Id) - Array.IndexOf(variables[var2Id].Current.Values, value2Id)) == 1;
        }

        public static void DisplaySolution(List<Permutation> perms)
        {
            foreach (var value in perms[0].Values)
            {
                switch (value)
                {
                    case 1:
                        Console.Write("Norweg");
                        break;
                    case 2:
                        Console.Write("Anglik");
                        break;
                    case 3:
                        Console.Write("Duńczyk");
                        break;
                    case 4:
                        Console.Write("Niemiec");
                        break;
                    case 5:
                        Console.Write("Szwed");
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();

            foreach (var value in perms[1].Values)
            {
                switch (value)
                {
                    case 1:
                        Console.Write("czerwony");
                        break;
                    case 2:
                        Console.Write("zielony");
                        break;
                    case 3:
                        Console.Write("biały");
                        break;
                    case 4:
                        Console.Write("żólty");
                        break;
                    case 5:
                        Console.Write("niebieski");
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();

            foreach (var value in perms[2].Values)
            {
                switch (value)
                {
                    case 1:
                        Console.Write("light");
                        break;
                    case 2:
                        Console.Write("bezfiltra");
                        break;
                    case 3:
                        Console.Write("mentolowe");
                        break;
                    case 4:
                        Console.Write("cygara");
                        break;
                    case 5:
                        Console.Write("fajka");
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();

            foreach (var value in perms[3].Values)
            {
                switch (value)
                {
                    case 1:
                        Console.Write("herbata");
                        break;
                    case 2:
                        Console.Write("mleko");
                        break;
                    case 3:
                        Console.Write("woda");
                        break;
                    case 4:
                        Console.Write("piwo");
                        break;
                    case 5:
                        Console.Write("kawa");
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();

            foreach (var value in perms[4].Values)
            {
                switch (value)
                {
                    case 1:
                        Console.Write("kot");
                        break;
                    case 2:
                        Console.Write("ptak");
                        break;
                    case 3:
                        Console.Write("pies");
                        break;
                    case 4:
                        Console.Write("koń");
                        break;
                    case 5:
                        Console.Write("smok");
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();
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
