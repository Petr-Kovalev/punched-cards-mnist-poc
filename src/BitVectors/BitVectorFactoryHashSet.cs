using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorFactoryHashSet: IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> activeBitIndices, int count)
        {
            return new BitVectorHashSet(activeBitIndices, count);
        }
    }
}