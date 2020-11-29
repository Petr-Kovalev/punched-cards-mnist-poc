using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorFactoryBitArray: IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> activeBitIndices, int count)
        {
            return new BitVectorBitArray(activeBitIndices, count);
        }
    }
}