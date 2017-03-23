using System;
using System.Collections.Generic;

namespace MyCollections
{
    /// <summary>
    /// Class derived from <see cref="SortedSet{T}"/> allowing equating and hashing based on content of sets.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class EquatableSortedSet<T> : SortedSet<T>, IEquatable<EquatableSortedSet<T>>
    {
        /// <summary>
        /// Compares this <see cref="EquatableSortedSet{T}"/> with <see cref="other"/> <see cref="EquatableSortedSet{T}"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True, if <see cref="other"/> is not null and sets are equal; False otherwise.</returns>
        public bool Equals(EquatableSortedSet<T> other)
        {
            return other != null && SetEquals(other);
        }

        /// <summary>
        /// Hashes this <see cref="EquatableSortedSet{T}"/> based on its content.
        /// </summary>
        /// <returns>Hash code.</returns>
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
