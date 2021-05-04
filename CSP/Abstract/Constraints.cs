using System;

namespace CSP.Abstract
{
    public class Constraint
    {
        public Func<bool> Check { get; set; }
    }

    public class UnaryConstraint<T> : Constraint
    {
        public IVariable<T> Variable { get; set; }
    }

    public class BinaryConstraint<T> : Constraint
    {
        public IVariable<T> VariableA { get; set; }
        public IVariable<T> VariableB { get; set; }

        //public Func<T, T, bool> CheckExplicit { get; set; }
    }
}
