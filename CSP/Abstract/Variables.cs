using System.Collections.Generic;

namespace CSP.Abstract
{
    public interface IVariable<T>
    {
        public T Current { get; set; }
        public List<T> Domain { get; set; }
        
        // Forward checking
        public bool[] DomainMask { get; set; }

        public List<T> MaskedDomain
        {
            get
            {
                List<T> res = new List<T>();
                for (int i = 0; i < DomainMask.Length; i++)
                {
                    if (DomainMask[i])
                    {
                        res.Add(Domain[i]);
                    }
                }

                return res;
            }
        }

        public void ResetDomain()
        {
            for (int i = 0; i < DomainMask.Length; i++)
            {
                DomainMask[i] = true;
            }
        }

        public void Prune(T item)
        {
            int idx = Domain.IndexOf(item);
            DomainMask[idx] = false;
        }
    }
}
