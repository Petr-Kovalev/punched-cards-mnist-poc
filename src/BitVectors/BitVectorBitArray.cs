using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorBitArray : BitVector
    {
        private readonly BitArray _bitArray;

        internal BitVectorBitArray(IEnumerable<int> activeBitIndices, int count)
        {
            _bitArray = new BitArray(count);

            foreach (var activeBitIndex in activeBitIndices)
            {
                _bitArray[activeBitIndex] = true;
            }
        }

        public override int Count => _bitArray.Count;

        public override IBitVector Punch(IEnumerable<int> indices)
        {
            var indicesList = indices.ToList();
            return new BitVectorBitArray(PunchInternal(indicesList), indicesList.Count);
        }

        public override bool IsBitActive(int index)
        {
            return _bitArray[index];
        }

        public override int AndCardinality(IBitVector bitVector)
        {
            if (Count != bitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            var bitVectorBitArray = (BitVectorBitArray) bitVector;

            var cardinality = 0;

            for (var bitIndex = 0; bitIndex < Count; bitIndex++)
            {
                if (IsBitActive(bitIndex) && bitVectorBitArray.IsBitActive(bitIndex))
                {
                    cardinality++;
                }
            }

            return cardinality;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BitVectorBitArray other) ||
                Count != other.Count)
            {
                return false;
            }

            for (var bitIndex = 0; bitIndex < Count; bitIndex++)
            {
                if (IsBitActive(bitIndex) != other.IsBitActive(bitIndex))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 17;

            unchecked
            {
                for (var bitIndex = 0; bitIndex < Count; bitIndex++)
                {
                    if (IsBitActive(bitIndex))
                    {
                        hashCode = hashCode * 23 + bitIndex.GetHashCode();
                    }
                }
            }

            return hashCode;
        }
    }
}