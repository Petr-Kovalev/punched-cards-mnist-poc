using System;
using System.Collections.Generic;
using System.Linq;
using CRoaring;

namespace PunchedCards
{
    internal sealed class BitVectorRoaringBitmap : IBitVector
    {
        private readonly RoaringBitmap _roaringBitmap;

        private BitVectorRoaringBitmap(RoaringBitmap roaringBitmap, int count)
        {
            _roaringBitmap = roaringBitmap;
            Count = count;
        }

        internal BitVectorRoaringBitmap(IEnumerable<int> indices, int count)
        {
            var indicesArray = indices.Select(i => (uint) i).ToArray();
            _roaringBitmap = new RoaringBitmap();
            _roaringBitmap.AddMany(indicesArray, 0U, (uint) indicesArray.Length);
            _roaringBitmap.Optimize();

            Count = count;
        }

        internal BitVectorRoaringBitmap(IEnumerable<bool> booleanEnumerable)
        {
            uint index = 0;

            var indices = new List<uint>();
            foreach (var bit in booleanEnumerable)
            {
                if (bit)
                {
                    indices.Add(index);
                }

                index++;
            }

            var indicesArray = indices.ToArray();
            _roaringBitmap = new RoaringBitmap();
            _roaringBitmap.AddMany(indicesArray, 0U, (uint) indicesArray.Length);
            _roaringBitmap.Optimize();

            Count = (int) index;
        }

        public int Count { get; }

        public IBitVector Punch(IEnumerable<int> indices)
        {
            var indicesList = indices.ToList();
            return new BitVectorRoaringBitmap(Punch(_roaringBitmap, indicesList), indicesList.Count);
        }

        public int AndCardinality(IBitVector bitVector)
        {
            if (Count != bitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            return (int) _roaringBitmap.AndCardinality(((BitVectorRoaringBitmap) bitVector)._roaringBitmap);
        }

        public override bool Equals(object obj)
        {
            return obj is BitVectorRoaringBitmap other &&
                   Count.Equals(other.Count) &&
                   _roaringBitmap.Equals(other._roaringBitmap);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            unchecked
            {
                hash = hash * 23 + Count.GetHashCode();

                foreach (var bitIndex in _roaringBitmap)
                {
                    hash = hash * 23 + bitIndex.GetHashCode();
                }
            }

            return hash;
        }

        private static RoaringBitmap Punch(RoaringBitmap roaringBitmap, IEnumerable<int> indices)
        {
            var punch = new RoaringBitmap();

            uint currentIndex = 0;

            foreach (uint index in indices)
            {
                if (roaringBitmap.Contains(index))
                {
                    punch.Add(currentIndex);
                }

                currentIndex++;
            }

            punch.Optimize();

            return punch;
        }
    }
}