#define ENABLE_LOGS

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

        private int counter = 0;

        public void Initialize()
        {
            DefineVariables();
            DefineConstraints();
        }

        public void RunBacktracking()
        {
            Console.WriteLine(Ac3());
            counter = 0;
            FindSolutionByBacktracking(0);
#if ENABLE_LOGS
            Console.WriteLine(counter);
#endif
            counter = 0;
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
                counter++;
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
        public void RunAc3()
        {
            Ac3();
        }

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
                }
            }

            while (workList.Count > 0)
            {
                var arc = workList[0];
                workList.RemoveAt(0);
                if (ArcReduce(arc))
                {
                    Console.WriteLine("XDDDD");
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
            counter = 0;
            FindSolutionForwardChecking(0, null);
#if ENABLE_LOGS
            Console.WriteLine(counter);
#endif
            counter = 0;
        }

        // int => indeks variabla, int - indeks zmiennej z dziedziny
        protected bool FindSolutionForwardChecking(int variableIndex, List<(int,int)> prunedValues)
        {
            foreach (var variable in Variables)
            {
                variable.DomainMask = new bool[variable.Domain.Count];
                for (int i = 0; i < variable.DomainMask.Length; i++)
                {
                    variable.DomainMask[i] = true;
                }
            }

            if (variableIndex == Variables.Count)
            {
                OnSolutionFound?.Invoke(Variables);
                return true;
            }

            foreach (var val in Variables[variableIndex].Domain)
            {
                Variables[variableIndex].Current = val;
                counter++;
                if (Constraints.All(con => con.Check.Invoke()))
                {
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

                    var prunedValuesForNextCall = new List<(int,int)>();
                    foreach (var pair in neighborVariables)
                    {
                        int idx = Variables.IndexOf(pair.Item1);
                        if (pair.Item1.Current == null)
                        {
                            for (int i = 0; i < pair.Item1.Domain.Count; i++)
                            {
                                if (!pair.Item1.DomainMask[i])
                                {
                                    pair.Item1.Current = pair.Item1.Domain[i];
                                    counter++;
                                    if (!pair.Item2.Check())
                                    {
                                        pair.Item1.DomainMask[i] = false;
                                        prunedValuesForNextCall.Add((idx, i));
                                    }
                                }
                            }

                            pair.Item1.Current = default;
                        }
                    }
                    // end of prune

                    var solutionFound = FindSolutionForwardChecking(variableIndex + 1, prunedValuesForNextCall);
                    if (solutionFound)
                    {
                        return true;
                    }
                }
                else
                {
                    Variables[variableIndex].Current = default;

                    // revert prune
                    if (prunedValues != null)
                    {
                        foreach (var pair in prunedValues)
                        {
                            Variables[pair.Item1].DomainMask[pair.Item2] = true;
                        }
                    }
                    // end of revert prune
                }
            }

            return false;
        }
    }
}
