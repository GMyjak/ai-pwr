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

        public void RunBacktracking()
        {
            DefineVariables();
            DefineConstraints();
            FindSolutionByBacktracking(0);
        }

        protected void FindSolutionByBacktracking(int variableIndex)
        {
            if (variableIndex == Variables.Count)
            {
                OnSolutionFound?.Invoke(Variables);
                return;
            }

            foreach (var val in Variables[variableIndex].Domain)
            {
                Variables[variableIndex].Current = val;
                if (Constraints.All(con => con.Check.Invoke()))
                {
                    FindSolutionByBacktracking(variableIndex + 1);
                }
                else
                {
                    Variables[variableIndex].Current = default;
                }
            }
        }
    }
}
