using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        int Count { get; }

        bool IsBitActive(int index);

        int AndCardinality(IBitVector bitVector);

        IBitVector Punch(IEnumerable<int> indices);
    }
}