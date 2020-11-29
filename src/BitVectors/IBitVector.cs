using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        int Count { get; }

        int AndCardinality(IBitVector bitVector);

        IBitVector Punch(IEnumerable<int> indices);
    }
}