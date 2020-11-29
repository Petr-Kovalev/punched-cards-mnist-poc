using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal sealed class BitVectorFactoryRoaringBitmap: IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> activeBitIndices, int count)
        {
            return new BitVectorRoaringBitmap(activeBitIndices, count);
        }
    }
}