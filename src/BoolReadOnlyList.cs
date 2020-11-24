using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards
{
    internal sealed class BoolReadOnlyList : IReadOnlyList<bool>
    {
        private readonly BitArray _bitArray;

        internal BoolReadOnlyList(IEnumerable<int> indices, int count)
        {
            _bitArray = new BitArray(GetBooleanArray(indices, count));
        }

        internal BoolReadOnlyList(IEnumerable<bool> booleanEnumerable)
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