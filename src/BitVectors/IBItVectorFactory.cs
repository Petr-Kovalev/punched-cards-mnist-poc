using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVectorFactory
    {
        IBitVector Create(IEnumerable<int> activeBitIndices, int count);
    }
}