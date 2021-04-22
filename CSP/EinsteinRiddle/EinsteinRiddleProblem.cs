using System;
using System.Collections.Generic;
using System.Linq;
using CSP.Abstract;

namespace CSP.EinsteinRiddle
{
    class EinsteinRiddleProblem : AbstractConstraintSatisfactionProblem<Permutation>
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

        protected override void DefineVariables()
        {
            Variables = new List<IVariable<Permutation>>();

            for (int i = 0; i < 5; i++)
            {
                Variables.Add(new Variable()
                {
                    Current = null,
                    Domain = Permutation.GetAllPermutations()
                });
            }
        }

        protected override void DefineConstraints()
        {
            Constraints = new List<Constraint>();

            // R1
            Constraints.Add(GenerateUnary(0, 1, 0));

            // R2
            Constraints.Add(GenerateBinary(0, 2, 1, 1));

            // R3
            Constraints.Add(new UnaryConstraint<Permutation>()
            {
                Check = () => Variables[1].Current == null || Array.IndexOf(Variables[1].Current.Values, 2) ==
                        Array.IndexOf(Variables[1].Current.Values, 3) - 1,
                Variable = Variables[1]
            });

            // R4
            Constraints.Add(GenerateBinary(0, 3, 3, 1));

            // R5
            Constraints.Add(GenerateBinaryNeighbor(2, 1, 4, 1));

            // R6
            Constraints.Add(GenerateBinary(1, 4, 2, 4));

            // R7
            Constraints.Add(GenerateBinary(0, 4, 2, 5));

            // R8
            Constraints.Add(GenerateUnary(3, 2, 2));

            // R9
            Constraints.Add(GenerateBinaryNeighbor(2, 1, 3, 3));

            // R10
            Constraints.Add(GenerateBinary(2, 2, 4, 2));

            // R11
            Constraints.Add(GenerateBinary(0, 5, 4, 3));

            // R12
            Constraints.Add(GenerateBinaryNeighbor(0, 1, 1, 5));

            // R13
            Constraints.Add(GenerateBinaryNeighbor(4, 4, 1, 4));

            // R14
            Constraints.Add(GenerateBinary(2, 3, 3, 4));

            // R15
            Constraints.Add(GenerateBinary(1, 2, 3, 5));
        }

        private Constraint GenerateUnary(int varId, int value, int index)
        {
            Constraint result = new UnaryConstraint<Permutation>()
            {
                Check = () => Variables[varId].Current == null || Variables[varId].Current.Values[index] == value,
                Variable = Variables[varId]
            };
            return result;
        }

        private Constraint GenerateBinary(int var1Id, int value1Id, int var2Id, int value2Id)
        {
            Constraint result = new BinaryConstraint<Permutation>()
            {
                Check = () => Variables[var1Id].Current == null || Variables[var2Id].Current == null ||
                              Array.IndexOf(Variables[var1Id].Current.Values, value1Id) ==
                              Array.IndexOf(Variables[var2Id].Current.Values, value2Id),
                VariableA = Variables[var1Id],
                VariableB = Variables[var2Id]
            };
            return result;
        }

        private Constraint GenerateBinaryNeighbor(int var1Id, int value1Id, int var2Id, int value2Id)
        {
            Constraint result = new BinaryConstraint<Permutation>()
            {
                Check = () => Variables[var1Id].Current == null || Variables[var2Id].Current == null ||
                              Math.Abs(Array.IndexOf(Variables[var1Id].Current.Values, value1Id) - Array.IndexOf(Variables[var2Id].Current.Values, value2Id)) == 1,
                VariableA = Variables[var1Id],
                VariableB = Variables[var2Id]
            };
            return result;
        }

        public static void DisplaySolution(List<IVariable<Permutation>> vars)
        {
            foreach (var value in vars[0].Current.Values)
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

            foreach (var value in vars[1].Current.Values)
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

            foreach (var value in vars[2].Current.Values)
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

            foreach (var value in vars[3].Current.Values)
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

            foreach (var value in vars[4].Current.Values)
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

    class Variable : IVariable<Permutation>
    {
        public Permutation Current { get; set; }
        public List<Permutation> Domain { get; set; }
        public bool[] DomainMask { get; set; }
    }

    public class Permutation
    {
        public int[] Values { get; set; }

        public static List<Permutation> GetAllPermutations()
        {
            var perms = GetAllPermutations(new List<int>() { 1, 2, 3, 4, 5 }, 5);
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
