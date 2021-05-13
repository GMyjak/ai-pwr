//#define ENABLE_LOGS

// Heuristics - only one will be used
#define H_DOM_SIZE
//#define H_MCV
//#define H_LCV

// Whether to use AC3 algo
//#define AC3

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSP.Abstract
{
    public abstract class AbstractConstraintSatisfactionProblem<T>
    {
        public List<IVariable<T>> Variables { get; set; }
        public List<Constraint> Constraints { get; set; }
        public Action<List<IVariable<T>>> OnSolutionFound { get; set; } = _ => { };

        protected abstract void DefineVariables();
        protected abstract void DefineConstraints();

        public int Counter = 0;

        public void Initialize()
        {
            DefineVariables();
            DefineConstraints();
        }

        public void RunBacktracking()
        {
#if AC3
            Ac3();
#endif
            Counter = 0;
            FindSolutionByBacktracking(0);
#if ENABLE_LOGS
            Console.WriteLine(Counter);
#endif
            //Counter = 0;
        }

        protected bool FindSolutionByBacktracking(int variableIndex)
        {
            if (variableIndex == Variables.Count)
            {
                OnSolutionFound?.Invoke(Variables);
                return true;
            }

            foreach (var val in Variables[variableIndex].Domain)
            {
                Variables[variableIndex].Current = val;
                Counter++;
                if (Constraints.All(con => con.Check.Invoke()))
                {
                    var solutionFound = FindSolutionByBacktracking(variableIndex + 1);
                    if (solutionFound)
                    {
                        return true;
                    }
                }
                else
                {
                    Variables[variableIndex].Current = default;
                }
            }

            return false;
        }

        // Pseudokod xDxD
        // https://en.wikipedia.org/wiki/AC-3_algorithm
        protected bool Ac3()
        {
            //List<(IVariable<T>, IVariable<T>)> workList = new List<(IVariable<T>, IVariable<T>)>();
            List<BinaryConstraint<T>> workList = new List<BinaryConstraint<T>>();
            foreach (var constraint in Constraints)
            {
                var cast = constraint as BinaryConstraint<T>;
                if (cast != null)
                {
                    workList.Add(cast);
                    BinaryConstraint<T> reverseConstraint = new BinaryConstraint<T>()
                    {
                        Check = cast.Check,
                        VariableA = cast.VariableB,
                        VariableB = cast.VariableA
                    };
                    workList.Add(reverseConstraint);
                }
            }

            while (workList.Count > 0)
            {
                var arc = workList[0];
                workList.RemoveAt(0);
                if (ArcReduce(arc))
                {
                    if (arc.VariableA.Domain.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        Constraints.ForEach(c =>
                        {
                            var cast = c as BinaryConstraint<T>;
                            IVariable<T> other = null;
                            if (cast.VariableA == arc.VariableA)
                            {
                                other = cast.VariableB;
                            }
                            if (cast.VariableB == arc.VariableA)
                            {
                                other = cast.VariableA;
                            }

                            if (other != null && other != arc.VariableB)
                            {
                                workList.Add(cast);
                                BinaryConstraint<T> reverseConstraint = new BinaryConstraint<T>()
                                {
                                    Check = cast.Check,
                                    VariableA = cast.VariableB,
                                    VariableB = cast.VariableA
                                };
                                workList.Add(reverseConstraint);
                            }
                        });
                    }
                }
            }

            return true;
        }

        protected bool ArcReduce(BinaryConstraint<T> arc)
        {
            var x = arc.VariableA;
            var y = arc.VariableB;

            bool change = false;
            for (var i = 0; i < x.Domain.Count; i++)
            {
                var valueX = x.Domain[i];
                x.Current = valueX;

                bool satisfactionFound = false;
                foreach (var valueY in y.Domain)
                {
                    y.Current = valueY;
                    if (arc.Check())
                    {
                        satisfactionFound = true;
                        break;
                    }
                }

                if (!satisfactionFound)
                {
                    Console.WriteLine("Reduced xd");
                    x.Domain.Remove(valueX);
                    change = true;
                }

                y.Current = default;
            }

            x.Current = default;

            return change;
        }

        public void RunForwardChecking()
        {
#if AC3
            Ac3();
#endif

            foreach (var variable in Variables)
            {
                variable.DomainMask = new bool[variable.Domain.Count];
                for (int i = 0; i < variable.DomainMask.Length; i++)
                {
                    variable.DomainMask[i] = true;
                }
            }

            Counter = 0;
            FindSolutionForwardChecking(0);
#if ENABLE_LOGS
            Console.WriteLine(Counter);
#endif
            //Counter = 0;
        }

        // int => indeks variabla, int - indeks wartosci z dziedziny
        protected bool FindSolutionForwardChecking(int variableIndex)
        {
            var values = Variables[variableIndex].MaskedDomain;
            foreach (var val in values)
            {
                Variables[variableIndex].Current = val;
                Counter++;
                if (Constraints.All(con => con.Check.Invoke()))
                {
                    if (Variables.All(v => v.Current != null))
                    {
                        OnSolutionFound?.Invoke(Variables);
                        return true;
                    }

                    // prune
                    var neighborVariables = new List<(IVariable<T>, BinaryConstraint<T>)>();
                    Constraints.ForEach(c =>
                    {
                        var cast = c as BinaryConstraint<T>;
                        if (cast != null && cast.VariableA == Variables[variableIndex])
                        {
                            neighborVariables.Add((cast.VariableB, cast));
                        }

                        if (cast != null && cast.VariableB == Variables[variableIndex])
                        {
                            neighborVariables.Add((cast.VariableA, cast));
                        }
                    });
                    neighborVariables = neighborVariables.Where(v => v.Item1.Current == null).ToList();

                    var prunedValues = new List<(int,int)>();
                    foreach (var pair in neighborVariables)
                    {
                        int idx = Variables.IndexOf(pair.Item1);
                        var variable = Variables[idx];
                        if (variable.Current == null)
                        {
                            for (int i = 0; i < variable.Domain.Count; i++)
                            {
                                if (variable.DomainMask[i])
                                {
                                    variable.Current = variable.Domain[i];
                                    //counter++;
                                    if (!pair.Item2.Check())
                                    {
                                        variable.DomainMask[i] = false;
                                        prunedValues.Add((idx, i));
                                    }
                                    variable.Current = default;
                                }
                            }

                            pair.Item1.Current = default;
                        }
                    }
                    // end of prune

                    // HEURISTICS
                    int newIndex;
#if H_DOM_SIZE
                    var chosenVar = Variables
                        .Where(v => v.Current == null)
                        .OrderBy(v => v.MaskedDomain.Count)
                        .First();
                    newIndex = Variables.IndexOf(chosenVar);
#elif H_MCV
                    var chosenVar = Variables
                        .Where(v => v.Current == null)
                        .OrderBy(v => Constraints.Sum(c =>
                        {
                            var cast = c as BinaryConstraint<T>;
                            if (cast == null)
                            {
                                return 0;
                            }
                            else if (cast.VariableA == v || cast.VariableB == v)
                            {
                                return 1;
                            }

                            return 0;
                        }))
                        .First();
                    newIndex = Variables.IndexOf(chosenVar);
#elif H_LCV
                    var chosenVar = Variables
                        .Where(v => v.Current == null)
                        .OrderByDescending(v => Constraints.Sum(c =>
                        {
                            var cast = c as BinaryConstraint<T>;
                            if (cast == null)
                            {
                                return 0;
                            }
                            else if (cast.VariableA == v || cast.VariableB == v)
                            {
                                return 1;
                            }

                            return 0;
                        }))
                        .First();
                    newIndex = Variables.IndexOf(chosenVar);
#else
                    newIndex = variableIndex + 1;
#endif

                    var solutionFound = FindSolutionForwardChecking(newIndex);
                    if (solutionFound)
                    {
                        return true;
                    }
                    else
                    {
                        // revert prune
                        foreach (var pair in prunedValues)
                        {
                            Variables[pair.Item1].DomainMask[pair.Item2] = true;
                        }
                        // end of revert prune
                    }
                }
                else
                {
                    Variables[variableIndex].Current = default;
                }
            }

            Variables[variableIndex].Current = default;

            return false;
        }
    }
}
