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

        public IEnumerator<bool> GetEnumerator()
        {
            return GetBooleanEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _bitArray.Count;

        public bool this[int index] => _bitArray[index];

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

        private IEnumerable<bool> GetBooleanEnumerable()
        {
            for (var index = 0; index < _bitArray.Count; index++)
            {
                yield return _bitArray[index];
            }
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