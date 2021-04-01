using System;
using System.Collections.Generic;

namespace CSP.Abstract
{
    class IConstraint<T>
    {
        public Func<List<IVariable<T>>, bool> Constraint { get; set; }
    }
}
