using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal interface IBitVector
    {
        int Count { get; }

        int HammingDistance(IBitVector bitVector);

        IBitVector Punch(IEnumerable<int> indices);
    }
}