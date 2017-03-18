using System;
using System.Collections.Generic;

namespace MyCollections
{
    public class EquatableSortedSet<T> : SortedSet<T>, IEquatable<EquatableSortedSet<T>>
    {
        public bool Equals(EquatableSortedSet<T> other)
        {
            return other != null && SetEquals(other);
        }

        public override int GetHashCode()
        {
            int hc = 0;
            foreach (var x in this)
            {
                hc = unchecked(hc + EqualityComparer<T>.Default.GetHashCode(x));
            }
            return hc;
        }
    }
}
