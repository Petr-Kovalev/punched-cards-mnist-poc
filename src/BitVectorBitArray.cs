using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards
{
    internal sealed class BitVectorBitArray : IBitVector
    {
        private readonly BitArray _bitArray;

        private BitVectorBitArray(BitArray bitArray)
        {
            _bitArray = bitArray;
        }

        internal BitVectorBitArray(IEnumerable<int> indices, int count)
        {
            _bitArray = new BitArray(count);

            foreach (var index in indices)
            {
                _bitArray[index] = true;
            }
        }

        internal BitVectorBitArray(IEnumerable<bool> booleanEnumerable)
        {
            _bitArray = new BitArray(booleanEnumerable.ToArray());
        }

        public int Count => _bitArray.Count;

        public IBitVector Punch(IEnumerable<int> indices)
        {
            return new BitVectorBitArray(PunchBitArray(indices.ToList()));
        }

        public int AndCardinality(IBitVector bitVector)
        {
            var bitArray = ((BitVectorBitArray) bitVector)._bitArray;

            if (_bitArray.Count != bitArray.Count)
            {
                throw new Exception("Counts does not match!");
            }

            var cardinality = 0;

            for (var i = 0; i < _bitArray.Count; i++)
            {
                if (_bitArray[i] && bitArray[i])
                {
                    cardinality++;
                }
            }

            return cardinality;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BitVectorBitArray other) ||
                _bitArray.Count != other._bitArray.Count)
            {
                return false;
            }

            for (var i = 0; i < _bitArray.Length; i++)
            {
                if (_bitArray[i] != other._bitArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = 17;

            unchecked
            {
                foreach (var bit in _bitArray)
                {
                    hash = hash * 23 + bit.GetHashCode();
                }
            }

            return hash;
        }

        private BitArray PunchBitArray(IReadOnlyCollection<int> indices)
        {
            var result = new BitArray(indices.Count);

            var currentIndex = 0;

            foreach (var index in indices)
            {
                if (_bitArray[index])
                {
                    result[currentIndex] = true;
                }

                currentIndex++;
            }

            return result;
        }
    }
}