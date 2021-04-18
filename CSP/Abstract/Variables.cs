using System.Collections.Generic;

namespace CSP.Abstract
{
    public interface IVariable<T>
    {
        public T Current { get; set; }
        public List<T> Domain { get; set; }
    }
}
