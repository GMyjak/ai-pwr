using System;
using System.Collections.Generic;
using System.Text;

namespace CSP.Abstract
{
    class AbstractConstraintSatisfactionProblem<T>
    {
        public List<IVariable<T>> Variables { get; set; }
        public IDomain<T> Domain { get; set; }
        public List<IConstraint<T>> Constraints { get; set; }
    }
}
