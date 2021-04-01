using System;
using System.Collections.Generic;
using System.Text;

namespace CSP.Abstract
{
    public interface IDomain<T>
    {
        public List<T> Values { get; set; }
    }
}
