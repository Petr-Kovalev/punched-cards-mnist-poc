using System.Collections.Generic;

namespace PunchedCards
{
    internal sealed class BitVectorFactoryBitArray: IBitVectorFactory
    {
        public IBitVector Create(IEnumerable<int> indices, int count)
        {
            return new BitVectorBitArray(indices, count);
        }

        public IBitVector Create(IEnumerable<bool> booleanEnumerable)
        {
            return new BitVectorBitArray(booleanEnumerable);
        }
    }
}