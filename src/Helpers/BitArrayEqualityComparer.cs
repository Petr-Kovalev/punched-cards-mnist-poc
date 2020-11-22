using System.Collections;
using System.Collections.Generic;

namespace PunchedCards.Helpers
{
    internal sealed class BitArrayEqualityComparer: IEqualityComparer<BitArray>
    {
        internal static IEqualityComparer<BitArray> Instance = new BitArrayEqualityComparer();

        private BitArrayEqualityComparer()
        {
        }

        public bool Equals(BitArray x, BitArray y)
        {
            if (x == null || 
                y == null ||
                x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(BitArray obj)
        {
            var hash = 17;

            unchecked
            {
                for (var i = 0; i < obj.Count; i++)
                {
                    hash = hash * 23 + obj[i].GetHashCode();
                }
            }

            return hash;
        }
    }
}