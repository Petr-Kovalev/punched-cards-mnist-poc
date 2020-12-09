using System;
using System.Collections.Generic;
using System.Linq;
using CRoaring;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorRoaringBitmap : IBitVector
    {
        private readonly RoaringBitmap _roaringBitmap;

        internal BitVectorRoaringBitmap(IEnumerable<int> activeBitIndices, int count)
        {
            var activeBitIndicesArray = activeBitIndices.Select(i => (uint) i).ToArray();
            _roaringBitmap = new RoaringBitmap();
            _roaringBitmap.AddMany(activeBitIndicesArray, 0U, (uint) activeBitIndicesArray.Length);
            _roaringBitmap.Optimize();

            Count = count;
        }

        public int Count { get; }

        public bool IsBitActive(int index)
        {
            return _roaringBitmap.Contains((uint) index);
        }

        public int HammingDistance(IBitVector bitVector)
        {
            if (Count != bitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            return (int) _roaringBitmap.XorCardinality(((BitVectorRoaringBitmap) bitVector)._roaringBitmap);
        }

        public override bool Equals(object obj)
        {
            return obj is BitVectorRoaringBitmap other &&
                   Count.Equals(other.Count) &&
                   _roaringBitmap.Equals(other._roaringBitmap);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                hashCode = hashCode * 23 + Count.GetHashCode();

                foreach (var activeBitIndex in _roaringBitmap)
                {
                    hashCode = hashCode * 23 + activeBitIndex.GetHashCode();
                }
            }

            return hashCode;
        }
    }
}