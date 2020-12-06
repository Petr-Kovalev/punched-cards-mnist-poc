using System;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorHashSet : BitVector
    {
        private readonly HashSet<int> _hashSet;

        internal BitVectorHashSet(IEnumerable<int> activeBitIndices, int count)
        {
            _hashSet = new HashSet<int>(activeBitIndices);
            Count = count;
        }

        public override int Count { get; }

        public override int AndCardinality(IBitVector bitVector)
        {
            if (Count != bitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            var hashSet = ((BitVectorHashSet) bitVector)._hashSet;

            HashSet<int> minimumHashSet;
            HashSet<int> maximumHashSet;
            if (_hashSet.Count > hashSet.Count)
            {
                minimumHashSet = hashSet;
                maximumHashSet = _hashSet;
            }
            else
            {
                minimumHashSet = _hashSet;
                maximumHashSet = hashSet;
            }

            return minimumHashSet.Count(maximumHashSet.Contains);
        }

        public override IBitVector Punch(IEnumerable<int> indices)
        {
            var indicesList = indices.ToList();
            return new BitVectorHashSet(PunchInternal(indicesList), indicesList.Count);
        }

        public override bool IsBitActive(int index)
        {
            return _hashSet.Contains(index);
        }

        public override bool Equals(object obj)
        {
            return obj is BitVectorHashSet bitVectorHashSet &&
                   Count.Equals(bitVectorHashSet.Count) &&
                   _hashSet.SetEquals(bitVectorHashSet._hashSet);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in _hashSet)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}