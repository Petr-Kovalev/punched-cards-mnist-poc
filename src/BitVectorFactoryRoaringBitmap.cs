using System.Collections.Generic;

namespace PunchedCards
{
    internal sealed class BitVectorFactoryRoaringBitmap: IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> indices, int count)
        {
            return new BitVectorRoaringBitmap(indices, count);
        }

        public IBitVector Create(IEnumerable<bool> booleanEnumerable)
        {
            return new BitVectorRoaringBitmap(booleanEnumerable);
        }
    }
}