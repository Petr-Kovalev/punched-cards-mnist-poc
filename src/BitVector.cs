using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards
{
    internal sealed class BitVector : IBitVector
    {
        private readonly BitArray _bitArray;

        internal BitVector(IEnumerable<int> indices, int count)
        {
            _bitArray = new BitArray(GetBooleanArray(indices, count));
        }

        internal BitVector(IEnumerable<bool> booleanEnumerable)
        {
            _bitArray = new BitArray(booleanEnumerable.ToArray());
        }

        public int Count => _bitArray.Count;

        public bool this[int index] => _bitArray[index];

        public int AndCardinality(IBitVector bitVector)
        {
            if (_bitArray.Count != bitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            var bitArray = ((BitVector) bitVector)._bitArray;

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
            if (!(obj is BitVector other) ||
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

        private static bool[] GetBooleanArray(IEnumerable<int> indices, int count)
        {
            var result = new bool[count];

            foreach (var index in indices)
            {
                result[index] = true;
            }

            return result;
        }
    }
}