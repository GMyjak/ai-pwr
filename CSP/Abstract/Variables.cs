using System;
using System.Collections.Generic;
using System.Text;

namespace CSP.Abstract
{
    public interface IVariable<T>
    {
        public T Current { get; set; }
        public List<T> Domain { get; set; }
    }
}
