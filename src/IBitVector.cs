using System.Collections.Generic;

namespace PunchedCards
{
    internal interface IBitVector
    {
        int Count { get; }

        int AndCardinality(IBitVector bitVector);

        IBitVector Punch(IEnumerable<int> indices);
    }
}